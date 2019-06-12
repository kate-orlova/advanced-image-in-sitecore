using System;

namespace AdvancedImage.GlassMapper.Fields
{
    public class AdvanceImageField : Glass.Mapper.Sc.Fields.Image
    {
        public float CropX { get; set; }

        public float CropY { get; set; }

        public float FocusX { get; set; }

        public float FocusY { get; set; }

        public bool ShowFull { get; set; }
        public string GetFocalPointParameters(int maxWidth = 0, float cropFactor = 0)
        {
            if (Math.Abs(cropFactor) < 0.00000001 || maxWidth == 0)
            {
                return string.Empty;
            }

            return $"cx={this.CropX}&cy={this.CropY}&cw={maxWidth}&ch={Math.Ceiling(maxWidth / cropFactor)}";
        }
    }
}