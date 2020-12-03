<script lang="ts">
    import { Component, Prop, Mixins, Vue } from 'vue-property-decorator';
    import { Bar } from 'vue-chartjs-typescript';
    export interface ChartDataInterface {
        labels: string[];
        avg_duration: number[];
        except_count: number[];
        except_type: string[];
    }
    @Component({})
    export default class TestGraph4 extends Mixins(Bar) {
        @Prop({ default: {} }) protected readonly data!: ChartDataInterface;
        private mounted() {
            const chartData = {
                labels: [] as string[],
                datasets: [
                    {
                        label: 'Exceptions: Each Day over the Month',
                        backgroundColor: '#0078d4',
                        data: [] as number[],
                    },
                ],
                options: {
                    response: true,
                    maintainAspectRatio: false,
                    scales: {
                        xAxes: [{
                            scaleLabel: {
                                display: true,
                                labelString: 'Day',
                            },
                            stacked: true,
                        }],
                        yAxes: [{
                            scaleLabel: {
                                display: true,
                                labelString: 'Exception Count',
                            },
                            stacked: true,
                        }],
                    },
                },
            };
            for (let i = 0; i < this.data.labels.length; i++) {
                chartData.labels.push(this.data.labels[i]);
                chartData.datasets[0].data.push(this.data.except_count[i]);
            }
            this.renderChart(chartData, chartData.options);
        }
    }
</script>