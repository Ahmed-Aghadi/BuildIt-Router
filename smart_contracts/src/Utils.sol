// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.13;

// import "solmate/tokens/ERC1155.sol";
import {ERC1155} from "openzeppelin/token/ERC1155/ERC1155.sol";
import "solmate/utils/LibString.sol";
import "openzeppelin/utils/Strings.sol";
// import "solmate/auth/Owned.sol";
import "openzeppelin/access/Ownable.sol";
import "openzeppelin/metatx/ERC2771Context.sol";
import "./routerprotocol/contracts/IDapp.sol";
import "./routerprotocol/contracts/IGateway.sol";

contract Utils is ERC2771Context, ERC1155, Ownable, IDapp {
    error CrossChainNotSupported();
    error InvalidChain();
    error InsufficientBalance();
    string public baseUri;
    uint256 public utilCount;
    IGateway public gatewayContract;
    mapping(string => string) public chains;

    modifier supportCrossChain() {
        if (address(gatewayContract) == address(0)) {
            revert CrossChainNotSupported();
        }
        _;
    }

    constructor(
        string memory _baseUri,
        address trustedForwarder,
        address payable gatewayAddress
    ) ERC2771Context(trustedForwarder) ERC1155(_baseUri) {
        baseUri = _baseUri;
        gatewayContract = IGateway(gatewayAddress);
    }

    function setChain(
        string memory chain,
        string calldata addr
    ) public onlyOwner {
        chains[chain] = addr;
    }

    function setDappMetadata(string memory FeePayer) public onlyOwner {
        gatewayContract.setDappMetadata(FeePayer);
    }

    function setGateway(address gateway) external onlyOwner {
        gatewayContract = IGateway(gateway);
    }

    function crossChainTransfer(
        string calldata destinationChain, // chain ID
        uint tokenId,
        uint amount,
        bytes calldata requestMetadata
    ) public payable supportCrossChain {
        string memory destinationAddress = chains[destinationChain];
        if (bytes(destinationAddress).length == 0) {
            revert InvalidChain();
        }
        if (amount > balanceOf(_msgSender(), tokenId)) {
            revert InsufficientBalance();
        }
        _burn(_msgSender(), tokenId, amount);
        bytes memory payload = abi.encode(tokenId, amount, _msgSender());

        bytes memory requestPacket = abi.encode(destinationAddress, payload);

        gatewayContract.setDappMetadata(
            Strings.toHexString(uint256(uint160(_msgSender())), 20)
        );

        gatewayContract.iSend{value: msg.value}(
            1,
            0,
            string(""),
            destinationChain,
            requestMetadata,
            requestPacket
        );
    }

    function getRequestMetadata(
        uint64 destGasLimit,
        uint64 destGasPrice,
        uint64 ackGasLimit,
        uint64 ackGasPrice,
        uint128 relayerFees,
        uint8 ackType,
        bool isReadCall,
        bytes memory asmAddress
    ) public pure returns (bytes memory) {
        bytes memory requestMetadata = abi.encodePacked(
            destGasLimit,
            destGasPrice,
            ackGasLimit,
            ackGasPrice,
            relayerFees,
            ackType,
            isReadCall,
            asmAddress
        );
        return requestMetadata;
    }

    function iReceive(
        string memory requestSender,
        bytes memory packet,
        string memory srcChainId
    ) external override supportCrossChain returns (bytes memory) {
        require(msg.sender == address(gatewayContract), "only gateway");

        if (
            keccak256(abi.encodePacked(chains[srcChainId])) !=
            keccak256(abi.encodePacked(requestSender))
        ) {
            revert InvalidChain();
        }
        (uint tokenId, uint amount, address sender) = abi.decode(
            packet,
            (uint, uint, address)
        );
        _mint(sender, tokenId, amount, "");

        return "";
    }

    function iAck(
        uint256 requestIdentifier,
        bool execFlag,
        bytes memory execData
    ) external override {}

    function mintMore(uint id, uint amount) public onlyOwner {
        _mint(_msgSender(), id, amount, "");
    }

    function mint(uint256 amount) public onlyOwner {
        utilCount += 1;
        _mint(_msgSender(), utilCount, amount, "");
    }

    function uri(
        uint256 id
    ) public view virtual override returns (string memory) {
        return string(abi.encodePacked(baseUri, LibString.toString(id)));
    }

    function _msgSender()
        internal
        view
        virtual
        override(Context, ERC2771Context)
        returns (address sender)
    {
        return ERC2771Context._msgSender();
    }

    function _msgData()
        internal
        view
        virtual
        override(Context, ERC2771Context)
        returns (bytes calldata)
    {
        return ERC2771Context._msgData();
    }
}
