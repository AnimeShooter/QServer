namespace TNL.Data
{
    public enum NetClassMask
    {
        NetClassGroupGameMask      = 1 << NetClassGroup.NetClassGroupGame,
        NetClassGroupCommunityMask = 1 << NetClassGroup.NetClassGroupCommunity,
        NetClassGroupMasterMask    = 1 << NetClassGroup.NetClassGroupMaster,

        NetClassGroupAllMask       = (1 << NetClassGroup.NetClassGroupCount) - 1,
    };
}
