minichain
====

Subjects
----
  * __BLOCKCHAIN__
    * [Customize minichain](customize_chain.md)
    * [Create a hardfork patch](hardfork_update.md)
  * __CONTRACT__
    * [Basic usage](contract/readme.md)

Setup
----
__Node__<br>
A basic node which cannot create a block. 
```cs
var node = new EndpointNode();

// There's no `Start` method in `EndpointNode`.
```

__Miner__<br>
You can operate mining node if you want to create a block and get mining rewards. This type of node will consume lots of CPU resources.
```cs
var node = new Miner();

node.Start();
```

P2p
----
__Connect to peer__
```cs
node.peers.AddPeer("ws://localhost:9916");
```

Chain
----
__Current block (last confirmed block)__
```cs
var block = node.chain.currentBlock;

var blockNo = block.blockNo;
var nonce = block.nonce; // solution
var difficulty = block.difficulty; // a difficulty number used to mine this block.
var merkleRootHash = block.merkleRootHash;
```

__Retrive balance__
```cs
var currentBalance = node.chain.GetBalance("ADDRESS");

var balanceAtSpecificBlock = node.chain.GetBalanceInBlock("ADDRESS", "BLOCK_HASH");
```

Contract
----
__Deploy__
```cs
var tx = node.wallet.CreateDeployTransaction("PROGRAM", "CTOR_SIGNATURE");
var contractAddress = tx.receiverAddr;
node.SendTransaction(tx);
```

__Call method__
```cs
var tx = node.wallet.CreateCallTransaction(
  contractAddress, "METHOD_SIGNATURE", 
  new object[] { 10, 20 });
node.SendTransaction(tx);
```

__Read field data__
```cs
var val = node.chain.GetPublicField(contractAddress, "FIELD_SIGNATURE");
```

Wallet
----
__Import and Export__
```cs
var json = node.wallet.Export();

node.wallet.Import(json);
```

__Retrive balance__
```cs
var currentBalance = node.wallet.GetBalance();

var balanceAtSpecificBlock = node.wallet.GetBalanceInBlock("BLOCK_HASH");
```

__Create a signed transaction__
```cs
node.wallet.CreateSignedTransaction("RECEIVER_ADDR", VALUE_AMOUNT);
```
