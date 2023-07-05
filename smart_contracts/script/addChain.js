const { ethers, network } = require("hardhat");
const hre = require("hardhat");

async function addChain() {
    accounts = await hre.ethers.getSigners();
    deployer = accounts[0];
    const chainId = network.config.chainId;
    console.log("Chain ID : " + chainId);
    console.log("Creating Utils contract");
    const utilsContractFactory = await hre.ethers.getContractFactory("Utils");

    const utilsAddressMumbai = "0x6D84F8C0386Bd77591EFDefb2CB40B6E141cbbA5";
    const utilsAddressMantleTestnet =
        "0xA82f14980559567F6d3B4DFFa3531045F1eDa522";

    let utilsContract;

    if (chainId == 80001) {
        utilsContract = await utilsContractFactory.attach(utilsAddressMumbai);
    } else if (chainId == 5001) {
        utilsContract = await utilsContractFactory.attach(
            utilsAddressMantleTestnet
        );
    }

    console.log("Utils contract created");
    console.log("Connecting user to Utils contract");
    const utils = await utilsContract.connect(deployer);
    console.log("User connected to Utils contract");

    let tx;

    if (chainId == 80001) {
        tx = await utils.setChain("5001", utilsAddressMantleTestnet);
    } else if (chainId == 5001) {
        tx = await utils.setChain("80001", utilsAddressMumbai);
    }

    console.log("----------------------------------");
    console.log(tx);
    const response = await tx.wait();
    console.log("----------------------------------");
    console.log(response);
    // console.log("address of entry : " + response.events[0].data)
}

addChain()
    .then(() => process.exit(0))
    .catch((error) => {
        console.error(error);
        process.exit(1);
    });
