Hardfork update
====

In this section, describes how to write hard-fork update codes in already operating blockchain.<br>
You need to read [this article](https://github.com/pjc0247/minichain/blob/master/docs/customize_chain.md) first.
<br><br>
Most important this is your code MUST be compatible with both version before and after update since old blocks also can be validated later.
See the examples below:

Change block reward after Nth block
----
If you think current block reward is too much and want to reduce it in the future, you can write code like below:
```cs
private static readonly int M2_Hardfork_BlockNo = 5000;

private static double M1_RewardFunction(int blockNo) 
{
    return 100000 - blockNo;
}
private static double M2_RewardFunction(int blockNo)
{
    return (100000 - blockNo) * 0.75; // x0.75 to reduce block reward
}

public static double CalcBlockReward(int blockNo)
{
    if (blockNo >= M2_Hardfork_BlockNo)
        return M2_RewardFunction(blockNo);
    else 
        return M1_RewardFunction(blockNo);
}
```

Change hashing algorhthm after Nth block
----
```cs
private static readonly int M3_Hardfork_BlockNo = 9000;

private static string M1_GetBlockHash(Block block) {
    return Hash.Sha1(block);
}
private static string M3_GetBlockHash(Block block) {
    return Hash.Sha512(block);
}

public static bool IsValidBlockLight(Block block, string nonce)
{
    string blockHash;
    
    if (block.blockNo >= M3_Hardfork_BlockNo)
       blockHash = M3_GetBlockHash(block);
    else 
       blockHash = M1_GetBlockHash(block);
       
    return VALIDATE_BLOCK(blockHash);
}
```
