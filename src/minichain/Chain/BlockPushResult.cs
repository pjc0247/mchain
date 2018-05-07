using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minichain
{
    internal enum BlockPushResultCode
    {
        OK,

        FromAnotherBranch,
        InvalidBlock
    }

    internal struct BlockPushResult
    {
        public BlockPushResultCode code;

        public string branchedBlockHash;

        public static BlockPushResult OK()
        {
            return new BlockPushResult(BlockPushResultCode.OK);
        }

        public BlockPushResult(BlockPushResultCode _code)
        {
            code = _code;
            branchedBlockHash = null;
        }
        public BlockPushResult(BlockPushResultCode _code, string _branchedBlockHash)
        {
            code = _code;
            branchedBlockHash = _branchedBlockHash;
        }
    }
}
