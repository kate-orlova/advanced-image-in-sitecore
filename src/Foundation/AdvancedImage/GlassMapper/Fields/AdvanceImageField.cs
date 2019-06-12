namespace AdvancedImage.GlassMapper.Fields
{
    public class AdvanceImageField : Glass.Mapper.Sc.Fields.Image
    {
        public float CropX { get; set; }

        public float CropY { get; set; }

        public float FocusX { get; set; }

        public float FocusY { get; set; }

        public bool ShowFull { get; set; }
    }
}