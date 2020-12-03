<script lang="ts">
    import { Component, Prop, Mixins, Vue } from 'vue-property-decorator';
    import { Line } from 'vue-chartjs-typescript';
    export interface ChartDataInterface {
        labels: string[];
        avg_duration: number[];
        except_count: number[];
    }
    @Component({})
    export default class RequestGraph3 extends Mixins(Line) {
        @Prop({ default: {} }) protected readonly data!: ChartDataInterface;
        private mounted() {
            const chartData = {
                labels: [] as string[],
                datasets: [
                    {
                        label: 'Average Request Duration: Each Day of the past Month',
                        backgroundColor: '#0078d4',
                        data: [] as number[],
                        fill: false,
                        lineTension: 0.0,
                    },
                ],
                options: {
                    response: true,
                    maintainAspectRatio: false,
                    scales: {
                        yAxes: [{
                            scaleLabel: {
                                display: true,
                                labelString: 'Time in mSec',
                            },
                        }],
                        xAxes: [{
                            scaleLabel: {
                                display: true,
                                labelString: 'Day',
                            },
                        }],
                    },
                },
            };
            for (let i = 0; i < this.data.labels.length; i++) {
                chartData.labels.push(this.data.labels[i]);
                chartData.datasets[0].data.push(this.data.avg_duration[i]);
            }
            this.renderChart(chartData, chartData.options);
        }
    }
</script>