namespace BimIshou.Services;
public class StorageProjectInfoService : StorageProjectInfoFactory<int>
{
    public override Guid Guid => new("7872E040-861A-47E4-AD50-8DFB68B862D9");
    public override string FieldName => "Int";
    public override string SchemaName => "StorageDimOffset";
    public override string VendorId => "ricaun";
}
