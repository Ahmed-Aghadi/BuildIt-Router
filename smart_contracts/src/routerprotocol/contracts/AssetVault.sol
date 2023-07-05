//SPDX-License-Identifier: MIT
pragma solidity >=0.8.0 <0.9.0;

import "openzeppelin/token/ERC20/presets/ERC20PresetMinterPauser.sol";
import "openzeppelin/security/ReentrancyGuard.sol";

contract AssetVault is ReentrancyGuard {
    address public immutable gateway;
    ERC20PresetMinterPauser public immutable routeToken;

    constructor(address gatewayAddress, address routeTokenAddress) {
        gateway = gatewayAddress;
        routeToken = ERC20PresetMinterPauser(routeTokenAddress);
    }

    event AssetBurned(uint256 amount, address indexed sender);
    event AssetMinted(uint256 amount, address indexed recipient);

    /// @notice function to deposit route token
    /// @param amount is the qty of Route token to burn
    /// @param caller is invoker of gateway contract
    function deposit(uint256 amount, address caller) external {
        require(msg.sender == gateway, "!Gateway");
        routeToken.burnFrom(caller, amount); // it any failure it will revert
        emit AssetBurned(amount, caller);
    }

    /// @notice function to withdraw route token
    /// @param amount is the qty of Route token to mint
    /// @param recipient is the address where router token will be transfered
    function handleWithdraw(uint256 amount, address recipient) external {
        require(msg.sender == gateway, "!Gateway");
        routeToken.mint(recipient, amount);
        emit AssetMinted(amount, recipient);
    }
}
