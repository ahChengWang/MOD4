using MOD4.Web.Enum;

namespace MOD4.Web.Repostory.Dao
{
    public class AccessFabOrderDetailDao
    {
        public int Sn { get; set; }
        public int AccessFabOrderSn { get; set; }
        public string GuestUnit { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string GuestPhone { get; set; }
        public string JobId { get; set; }
        public string Name { get; set; }
        public ClotheSizeEnum ClotheSizeId { get; set; }
        public ShoesSizeEnum ShoesSizeId { get; set; }
    }
}
