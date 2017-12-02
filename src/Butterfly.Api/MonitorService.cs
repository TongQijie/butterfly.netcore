using System.Threading.Tasks;
using Butterfly.MonitorManagement;
using Butterfly.ServiceModel;
using Guru.AspNetCore.Attributes;

namespace Butterfly.Api
{
    [Api("Monitor")]
    public class MonitorService
    {
        private readonly IMonitorHandler _MonitorHandler;

        public MonitorService(IMonitorHandler monitorHandler)
        {
            _MonitorHandler = monitorHandler;
        }

        [ApiMethod("GetChart")]
        public ChartConfiguration GetChart()
        {
            return new ChartConfiguration()
            {
                type = "line",
                data = new ChartData()
                {
                    labels = new string[] { "January", "February", "March", "April", "May", "June", "July" },
                    datasets = new ChartDataset[]
                    {
                        new ChartDataset()
                        {
                            label = "Dataset",
                            backgroundColor = "rgb(255, 99, 132)",
                            borderColor = "rgb(255, 99, 132)",
                            data = new double[] { 10, 50, -10, -20, 30, 20, 40 },
                        },
                    },
                },
                options = new ChartOptions()
                {
                    responsive = true,
                    title = new ChartOptionTitle()
                    {
                        display = true,
                        text = "Line Chart",
                    },
                    tooltips = new ChartOptionTooltips()
                    {
                        mode = "index",
                        intersect = false,
                    },
                    hover = new ChartOptionHover()
                    {
                        mode = "nearest",
                        intersect = true,
                    },
                    scales = new ChartOptionScales()
                    {
                        xAxes = new ChartOptionAxes[]
                        {
                            new ChartOptionAxes()
                            {
                                display = true,
                                scaleLabel = new ChartOptionScaleLabel()
                                {
                                    display = true,
                                    labelString = "Month"
                                },
                            },
                        },
                        yAxes = new ChartOptionAxes[]
                        {
                            new ChartOptionAxes()
                            {
                                display = true,
                                scaleLabel = new ChartOptionScaleLabel()
                                {
                                    display = true,
                                    labelString = "Value"
                                },
                            },
                        },
                    },
                },
            };
        }
    }
}