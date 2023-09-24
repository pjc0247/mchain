                                            _       _      _           _       
                                           (_)     (_)    | |         (_)      
                                  _ __ ___  _ _ __  _  ___| |__   __ _ _ _ __  
                                 | '_ ` _ \| | '_ \| |/ __| '_ \ / _` | | '_ \ 
                                 | | | | | | | | | | | (__| | | | (_| | | | | |
                                 |_| |_| |_|_|_| |_|_|\___|_| |_|\__,_|_|_| |_|
                                
                                          Minimal implementation of blockchain
                                                             Written in CSharp
                                                             pjc0247@naver.com
      

A fork of [minichain](https://github.com/pjc0247/minichain/tree/minimal-impl) which implements more advanced features such as VM and SmartContract stuffs.<br>
(minichain project pursues the minimal codes, That's why I made another project.)

<img src="https://github.com/pjc0247/minichain/raw/master/prev.gif" />

__[Documentation](docs/)__<br>

You can learn
----
  * P2P communication between nodes
  * How to create(mine) a new block and distribute to chain
  * Consensus based on `Proof of work`
  * Store/Retrive data(state) into blockchain
  * Signing transaction (Using asymmetric key validation)
  * Execute a program on blockchain network via `minidity`
  
__You can also learn (See example projects)__
  * Create a hardfork update to ongoing blockchain.
  * Bad node attack 

Sub repositories
----
* [Demonstrate 51% attack on blockchain network](https://github.com/pjc0247/minichain_51attack_demo)
* [State DB implementation](https://github.com/pjc0247/minichain_state_db)
* [coldog - offline wallet solution for mchain](https://github.com/pjc0247/coldog)

Specification
----
* __Consensus__
  * Proof of Work
  
* __Block Structure__

* __Block Validation__
  * `txs` must be a non empty array (except genesis-block)
  * `txs[0]` must be a reward transaction.
  * Check the block has valid minerAddress
  * Check the block proper difficulty
  * Check the nonce with block difficulty
