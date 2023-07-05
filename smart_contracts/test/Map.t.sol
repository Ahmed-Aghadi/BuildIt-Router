// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.13;

import "forge-std/Test.sol";
import "forge-std/console.sol";
import "../src/Map.sol";
import "../src/Utils.sol";
import "../src/Forwarder.sol";

import {ERC1155TokenReceiver} from "solmate/tokens/ERC1155.sol";

contract MapTest is Test {
    Map public map;
    Utils public utils;
    Forwarder public forwarder;
    string public mapBaseUri;
    string public utilsBaseUri;
    uint256 size;
    uint256 perSize;
    uint256 utilCount;
    uint256 utilAmount;

    bytes4 constant ERC1155_RECEIVED = 0xf23a6e61;
    bytes4 constant ERC1155_BATCH_RECEIVED = 0xbc197c81;

    function setUp() public {
        mapBaseUri = "https://example1.com/";
        utilsBaseUri = "https://example2.com/";
        size = 15;
        perSize = 5;
        utilCount = 3;
        utilAmount = 1000;
        forwarder = new Forwarder();
        utils = new Utils(
            utilsBaseUri,
            address(forwarder),
            payable(address(0))
        );
        map = new Map(size, 5, mapBaseUri, address(utils), address(forwarder));
        for (uint256 i = 0; i < utilCount; i++) {
            utils.mint(utilAmount);
        }
    }

    function onERC1155Received(
        address,
        address,
        uint256,
        uint256,
        bytes memory
    ) public virtual returns (bytes4) {
        return ERC1155TokenReceiver.onERC1155Received.selector;
    }

    function onERC1155BatchReceived(
        address,
        address,
        uint256[] memory,
        uint256[] memory,
        bytes memory
    ) public virtual returns (bytes4) {
        return ERC1155TokenReceiver.onERC1155BatchReceived.selector;
    }

    function testMint(uint256 x, uint256 y) public {
        vm.assume(x < size / perSize);
        vm.assume(y < size / perSize);
        uint id = map.mint(x, y);
        assertEq(map.ownerOf(id), address(this));
    }

    function testPlaceItem(
        uint256 x,
        uint256 y,
        uint256 utilId1,
        uint256 utilId2
    ) public {
        vm.assume(utilId1 != 0 && utilId2 != 0);
        vm.assume(utilId1 != utilId2);
        vm.assume(utilId1 <= utilCount && utilId2 <= utilCount);
        x = x % size;
        y = y % size;
        console.log("utilId1", utilId1);
        console.log("utilId2", utilId2);
        map.mint(x / perSize, y / perSize);
        utils.setApprovalForAll(address(map), true);
        uint256 balanceBefore1 = utils.balanceOf(address(this), utilId1);
        map.placeItem(x, y, utilId1);
        assertEq(map.map(x, y), utilId1, "1");
        uint256 balanceAfter1 = utils.balanceOf(address(this), utilId1);
        assertEq(balanceAfter1, balanceBefore1 - 1, "2");
        balanceBefore1 = utils.balanceOf(address(this), utilId1);
        uint256 balanceBefore2 = utils.balanceOf(address(this), utilId2);
        map.placeItem(x, y, utilId2);
        assertEq(map.map(x, y), utilId2, "3");
        uint256 balanceAfter2 = utils.balanceOf(address(this), utilId2);
        balanceAfter1 = utils.balanceOf(address(this), utilId1);
        assertEq(balanceAfter1, balanceBefore1 + 1, "4");
        assertEq(balanceAfter2, balanceBefore2 - 1, "5");
    }

    function testRemoveItem(uint256 x, uint256 y, uint256 utilId) public {
        vm.assume(utilId != 0);
        vm.assume(utilId <= utilCount);
        x = x % size;
        y = y % size;
        map.mint(x / perSize, y / perSize);
        utils.setApprovalForAll(address(map), true);
        map.placeItem(x, y, utilId);
        uint256 balanceBefore = utils.balanceOf(address(this), utilId);
        map.removeItem(x, y);
        assertEq(map.map(x, y), 0, "1");
        uint256 balanceAfter = utils.balanceOf(address(this), utilId);
        assertEq(balanceAfter, balanceBefore + 1, "2");
    }

    function testBaseUri(uint256 tokenId) public {
        string memory uri = map.tokenURI(tokenId);
        assertEq(uri, string.concat(mapBaseUri, vm.toString(tokenId)));
    }

    function testPlaceItems() public {
        map.mint(0, 1);
        uint256[] memory x = new uint256[](3);
        x[0] = 2;
        x[1] = 3;
        x[2] = 4;
        uint256[] memory y = new uint256[](3);
        y[0] = 7;
        y[1] = 8;
        y[2] = 9;
        uint256[] memory utilId = new uint256[](3);
        utilId[0] = 3;
        utilId[1] = 2;
        utilId[2] = 1;
        // console.log("first x: ", x[0] / perSize);
        // console.log("first y: ", y[0] / perSize);
        // console.log("second x: ", x[1] / perSize);
        // console.log("second y: ", y[1] / perSize);
        // console.log("third x: ", x[2] / perSize);
        // console.log("third y: ", y[2] / perSize);
        utils.setApprovalForAll(address(map), true);
        map.updateItems(x, y, utilId);
        assertEq(map.map(2, 7), 3);
        assertEq(map.map(3, 8), 2);
        assertEq(map.map(4, 9), 1);
    }
}
