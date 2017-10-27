namespace Butterfly.ServiceModel
{
    public class ChartConfiguration
    {
        public string type { get; set; }

        public ChartData data { get; set; }

        public ChartOptions options { get; set; }
    }

    public class ChartData
    {
        public string[] labels { get; set; }

        public ChartDataset[] datasets { get; set; }
    }

    public class ChartDataset
    {
        public string label { get; set; }

        public string backgroundColor { get; set; }

        public string borderColor { get; set; }

        public double[] data { get; set; }

        public bool fill { get; set; }
    }

    public class ChartOptions
    {
        public bool responsive { get; set; }

        public ChartOptionTitle title { get; set; }

        public ChartOptionTooltips tooltips { get; set; }

        public ChartOptionHover hover { get; set; }

        public ChartOptionScales scales { get; set; }
    }

    public class ChartOptionTitle
    {
        public bool display { get; set; }

        public string text { get; set; }
    }

    public class ChartOptionTooltips
    {
        public string mode { get; set; }

        public bool intersect { get; set; }
    }

    public class ChartOptionHover
    {
        public string mode { get; set; }

        public bool intersect { get; set; }
    }

    public class ChartOptionScales
    {
        public ChartOptionAxes[] xAxes { get; set; }

        public ChartOptionAxes[] yAxes { get; set; }
    }

    public class ChartOptionAxes
    {
        public bool display { get; set; }

        public ChartOptionScaleLabel scaleLabel { get; set; }
    }

    public class ChartOptionScaleLabel
    {
        public bool display { get; set; }

        public string labelString { get; set; }
    }
}