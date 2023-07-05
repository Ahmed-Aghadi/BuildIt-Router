BuildIt is a metaverse project developed for the hackathon. It provides users with the ability to own virtual land within a map, place items on the land they own, and even sell the land to other users. The land is represented as ERC721 tokens, while the items are represented as ERC1155 tokens. All interactions within the metaverse are secured by smart contracts.

## Installation

Go to `smart_contracts` folder and install foundry and hardhat.

```bash
cd smart_contracts/
forge init
npm install
```

To compile smart contracts:

```bash
forge compile --skip test --skip script

# Or If you want to compile test files

forge compile --via-ir
```

To run test on smart contracts:

```bash
forge test --via-ir
```

To deploy smart contracts:

```bash
npx hardhat deploy --network $ChainName

# example: npx hardhat deploy --network ethereum
```

Note: Don't forget to make .env file, refer .env.example file.

## Smart Contracts ( Mantle Testnet )

| Contract                                                                                                        | Explorer Link                                                                                                                        |
| --------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| [Map.sol](https://github.com/Ahmed-Aghadi/BuildIt-Router/blob/main/smart_contracts/src/Map.sol)                 | [0x798f13eB1B9702349372C2c01bcbbcDc2B450Dc1](https://explorer.testnet.mantle.xyz/address/0x798f13eB1B9702349372C2c01bcbbcDc2B450Dc1) |
| [Utils.sol](https://github.com/Ahmed-Aghadi/BuildIt-Router/blob/main/smart_contracts/src/Utils.sol)             | [0xA82f14980559567F6d3B4DFFa3531045F1eDa522](https://explorer.testnet.mantle.xyz/address/0xA82f14980559567F6d3B4DFFa3531045F1eDa522) |
| [Faucet.sol](https://github.com/Ahmed-Aghadi/BuildIt-Router/blob/main/smart_contracts/src/Faucet.sol)           | [0x87B90CB7C8f19f110F6920Eaa1b989bE74d97027](https://explorer.testnet.mantle.xyz/address/0x87B90CB7C8f19f110F6920Eaa1b989bE74d97027) |
| [Marketplace.sol](https://github.com/Ahmed-Aghadi/BuildIt-Router/blob/main/smart_contracts/src/Marketplace.sol) | [0x2dc5C39c1e93f71881e3B7bB1Ed6047adf154961](https://explorer.testnet.mantle.xyz/address/0x2dc5C39c1e93f71881e3B7bB1Ed6047adf154961) |

## Smart Contracts ( Polygon Mumbai )

| Contract                                                                                                        | Explorer Link                                                                                                                   |
| --------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| [Map.sol](https://github.com/Ahmed-Aghadi/BuildIt-Router/blob/main/smart_contracts/src/Map.sol)                 | [0xd6df3B609A759c4cF06e6B0Bb95BE5276dB79C8D](https://mumbai.polygonscan.com/address/0xd6df3B609A759c4cF06e6B0Bb95BE5276dB79C8D) |
| [Utils.sol](https://github.com/Ahmed-Aghadi/BuildIt-Router/blob/main/smart_contracts/src/Utils.sol)             | [0x6D84F8C0386Bd77591EFDefb2CB40B6E141cbbA5](https://mumbai.polygonscan.com/address/0x6D84F8C0386Bd77591EFDefb2CB40B6E141cbbA5) |
| [Faucet.sol](https://github.com/Ahmed-Aghadi/BuildIt-Router/blob/main/smart_contracts/src/Faucet.sol)           | [0xD231246547E9e44D6108266F4ba029877EB8246c](https://mumbai.polygonscan.com/address/0xD231246547E9e44D6108266F4ba029877EB8246c) |
| [Marketplace.sol](https://github.com/Ahmed-Aghadi/BuildIt-Router/blob/main/smart_contracts/src/Marketplace.sol) | [0x3a86b2450D5b9b0e747bCC3D82684F8Cc61830d6](https://mumbai.polygonscan.com/address/0x3a86b2450D5b9b0e747bCC3D82684F8Cc61830d6) |

## Router Protocol (Cross Chain Testing)

| Chain                            | RouterScan Link                                                                                          |
| -------------------------------- | -------------------------------------------------------------------------------------------------------- |
| Polygon Mumbai to Mantle Testnet | [Transfer of 10 Util Tokens](https://routerscan.io/crosschain/details?srcChainId=80001&eventNonce=53968) |

## Table of Contents

- [Inspiration](#inspiration)
- [What It Does](#what-it-does)
- [How We Built It](#how-we-built-it)
- [Challenges We Ran Into](#challenges-we-ran-into)
- [Accomplishments That We're Proud Of](#accomplishments-that-were-proud-of)
- [What We Learned](#what-we-learned)
- [What's Next for BuildIt](#whats-next-for-buildit)

## Inspiration

The inspiration behind BuildIt comes from the desire to create an immersive metaverse experience where users can explore, own, and customize virtual land. We wanted to empower users to express their creativity and engage with a virtual world where they have control over their own unique space.

## What It Does

BuildIt allows users to:

- Own virtual land within a map represented as ERC721 tokens.
- Place items on their owned land, such as buildings, roads, and other structures, represented as ERC1155 tokens.
- Sell their land to other users, transferring ownership and associated items.
- Connect their wallets (e.g., Metamask, Coinbase, WalletConnect) to interact with the metaverse.

When a user connects their wallet, the game fetches data from the smart contracts and highlights the portion of the map that the user owns. Users can then click the "Edit" button to place or remove items on their land. They have the option to cancel or confirm the changes, which updates the items in the appropriate locations. Smart contract checks ensure that users can only interact with the land they own.

In addition, BuildIt includes a marketplace where users can sell their land through direct listings or auctions. Chainlink automation can be utilized for auction listings, and if the chain supports Chainlink price feeds, the land can be sold in USD. The marketplace provides an easy and secure way for users to trade their land.

## How We Built It

BuildIt was built using the following technologies and tools:

- Unity: The game was developed using Unity and built for Webgl.
- Smart Contracts: Four smart contracts were developed using Foundry and Hardhat:
  - Map Contract: Responsible for the Lands in the Map, implemented as an ERC721 contract.
  - Utils Contract: Represents the items that can be placed on the land, implemented as an ERC1155 contract.
  - Faucet Contract: Allows users to obtain items for free initially. It is funded to provide items for judges and other participants.
  - Marketplace Contract: Facilitates land sales through direct listings and auctions.
- Map Size: The map size is determined in the smart contract, allowing the deployment of multiple maps with different sizes. The current deployment consists of a map with a size of 15 by 15 tiles, where each land is a 5 by 5 tile.
- Item Minting: Three items are minted in the Utils contract: road, house, and special item.
- Wallet Integration: Users can connect their wallets, such as Metamask, Coinbase, and WalletConnect, to interact with the metaverse.
- Gasless Transactions: All smart contracts implement ERC2771Context, enabling users to perform gasless transactions when the relayer is funded.
- Cross Chain Transfer: Router Protocol was used to transfer Util Items from one chain to another.

## Challenges We Ran Into

During the development of BuildIt, we encountered several challenges, including:

- Integrating Unity with the blockchain and ensuring secure and efficient interactions between the game and smart contracts.
- Implementing ERC721 and ERC1155 token standards and handling the transfer of ownership between users and their land/items.
- Optimizing gas usage and transaction costs in smart contract deployments.
- Developing a user-friendly interface and seamless wallet integration for a smooth user experience.

## Accomplishments That We're Proud Of

Throughout the development process, we achieved several accomplishments that we're proud of, including:

- Successfully integrating the Unity game engine with the Blockchain and smart contracts.
- Creating a metaverse where users can own virtual land and customize it with various items.
- Implementing a marketplace where users can buy and sell land securely through direct listings and auctions.
- Enabling gasless transactions for users by implementing ERC2771Context in all smart contracts.
- Conducting comprehensive testing, including fuzz testing, to ensure the stability and reliability of the application.

## What We Learned

The development of BuildIt provided us with valuable learning experiences, including:

- Gaining in-depth knowledge of integrating smart contracts with Unity.
- Understanding the intricacies of token standards like ERC721 and ERC1155.
- Optimizing gas usage and transaction costs in smart contract deployments.
- Enhancing user experience through seamless wallet integration and fetching data from smart contracts.

## What's Next for BuildIt

BuildIt is an ongoing project, and we have exciting plans for its future:

- Adding multiple maps with different sizes to expand the metaverse and accommodate more users.
- Conducting further research on gasless transactions to reduce transaction costs and improve user experience.
- Exploring cross-chain integrations to enable interoperability with other blockchain platforms.
- Enhancing the variety of items and customizations available to users.
- Engaging with the community to gather feedback and implement new features based on user suggestions.

We are dedicated to continuously improving and expanding BuildIt to create a vibrant and immersive metaverse experience for all users.
