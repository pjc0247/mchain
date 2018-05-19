Writing a contract
====

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
