minidity 
====

Mapping
----
```sol
class Mrc20 {
    public balances;

    def _ctor(_totalSupply) {
        balances[tx.sender()] = _totalSupply;
    }
}
```

Events
----
```sol
def transfer(address, amount) {
    /* ... */

    $transfer(tx.sender(), address, amount);
}
```