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
