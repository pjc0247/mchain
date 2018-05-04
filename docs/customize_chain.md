Customize minichain
====

https://github.com/pjc0247/minichain/blob/master/src/minichain/Chain/Consensus.cs

Block rewards
----
```cs
public static double CalcBlockReward(int blockNo)
{
    // You can implement dynamic block rewards
    //   like code below:
#if JUST_AN_EXAMPLE
     if (blockNo < 100)
         return 1;
     else if (blockNo < 200)
         return 0.5;
     else if (blockNo < 300)
         return 0.25;
     else return 0; // No reward after block#300
#endif

    // But, we just use static reward at this time.
    return 1;
}
```

Block difficulty
----
```cs
public static int CalcBlockDifficulty(int blockNo)
{
     // You can implement dynamic block difficulty
     //   like code below:
#if JUST_AN_EXAMPLE
     if (blockNo < 100)
         return 5;
     else if (blockNo < 200)
         return 6;
     else if (blockNo < 300)
         return 7;
     else return 8;
#endif

    // But, we just use static difficulty at this time.
    return 5;
}
```
