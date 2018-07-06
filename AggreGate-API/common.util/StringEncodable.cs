namespace com.tibbo.aggregate.common.util
{
    using System.Text;

    using com.tibbo.aggregate.common.datatable;

    public interface StringEncodable
    {
        StringBuilder encode(StringBuilder sb, ClassicEncodingSettings settings, bool isTransferEncode, int encodeLevel);
    }
}