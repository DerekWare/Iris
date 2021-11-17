using DerekWare.Collections;

namespace DerekWare.Net.HLS
{
    public class Segment : PlaylistEntry
    {
        public Segment(Playlist parent)
            : base(parent, null)
        {
            TreeNodeType = TreeNodeTypes.Leaf;
        }
    }
}
