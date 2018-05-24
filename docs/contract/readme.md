Writing a contract
====

__mchain__ contains a `mvm` which enables users can create an execute the various programs in blockchain system.

```
class MyFirstContract {
  def _ctor() {
  
  }
  
  def giveMeMoney() {
    Chain.transfer(tx.sender, 1.0);
  }
}
```

Internal calls
----


Deploy a contract
----
```cs
var program = "BASE64_ENCODED_PROGRAM";
var ctorSignature = "ContractName::_ctor";

var tx = node.wallet.CreateDeployTransaction(program, ctorSignature);
node.SendTransaction(tx);

var contractAddress = tx.receiverAddr;
```

Execute 
----
```cs
var methodSignature = "ContractName::MethodName";
var args = new object[] { 2, 3 };

var tx = node.wallet.CreateCallTransaction(contractAddress, methodSignature, args);
node.SendTransaction(tx);
```
