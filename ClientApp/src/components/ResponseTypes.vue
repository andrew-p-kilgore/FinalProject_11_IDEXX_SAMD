<script lang="ts">
    import { Component, Prop, Mixins, Vue } from 'vue-property-decorator';
    import { Pie, mixins } from 'vue-chartjs-typescript';
    const { reactiveProp } = mixins;

    export interface ChartDataInterface {
        labels: string[];
        avg_duration: number[];
        except_count: number[];
        percent_responses: number[];
    }

    @Component({
        mixins: [reactiveProp],
    })
    export default class ResponseTypes extends Mixins(Pie) {
        @Prop({ default: {} }) protected readonly data!: ChartDataInterface;
        @Prop({ default: {} }) protected readonly title!: string;

        private mounted() {
            // https://www.htmlcsscolor.com/ Color Names
            // Dictionary mapping response types to color hex codes
            const colorPairs: { [resultCode: string]: string} = {
                '0 [not sent in full]': '#993366',      // #993366: Approx. Lipstick (Red)
                '200': '#66ccff',                       // #66ccff: Approx. Maya Blue (Blue)
                '200 [not sent in full]': '#66ccff',    // #66ccff: Approx. Maya Blue (Blue)
                '201': '#67ffcc',                       // #67ffcc: Approx. Aquamarine (Green / Blue)
                '400': '#9999ff',                       // #9999ff: Approx. Portage (Indigo)
                '401': '#cc99ff',                       // #cc99ff: Approx. Mauve (Purple)
                '403': '#990033',                       // #990033: Approx. Carmine (Red)
                '404': '#ff9966',                       // #ff9966: Approx. Atomic Tangerine (Orange)
                '405': '#ffcc33',                       // #ffcc33: Approx. Sunglow (Gold)
                '406': '#ccffcc',                       // #ccffcc: Approx. Blue Romance (Green)
                '408': '#3399cc',                       // #3399cc: Approx. Summer Sky (Blue)
                '415': '#660000',                       // #660000: Approx. Kelly Green (Green)
                '429': '#3fbd82',
                '500': '#5cbb6b',
                '500 [not sent in full]': '#77b954',
                '502': '#91b43f',
                '503': '#abae2d',
                'Other': '#cccc99',                     // #cccc99: Approx. Green Mist (Cream / Gold)
            };

            const chartData = {
                labels: [] as string[],
                datasets: [
                    {
                        backgroundColor: [] as string[],
                        borderColor: [] as string[],
                        data: [] as number[],
                    },
                ],
            };

            // Strings used for indexes
            const labelString = 'labels';
            const indexString = 'index';
            const datasetString = 'datasets';
            const dataString = 'data';
            const otherString = 'Other';

            for (let i = 0; i < this.data.labels.length; i++) {
                chartData.labels.push(this.data.labels[i]);
                chartData.datasets[0].data.push(this.data.percent_responses[i]);

                // Use the 'Other' color for the combined category
                if (this.data.labels[i].includes(otherString)) {
                    chartData.datasets[0].backgroundColor.push(colorPairs[otherString]);
                    chartData.datasets[0].borderColor.push(colorPairs[otherString]);
                } else {
                    // Use the mapped colors
                    chartData.datasets[0].backgroundColor.push(colorPairs[chartData.labels[i]]);
                    chartData.datasets[0].borderColor.push(colorPairs[chartData.labels[i]]);
                }
            }

            this.renderChart(chartData, {
                response: true,
                maintainAspectRatio: false,
                title: {
                    display: true,
                    text: this.title,
                },
                // Edit the pie chart tooltip to show percent sign
                tooltips: {
                    callbacks: {
                        label(tooltipItem: any, data: any) {
                            return data[labelString][tooltipItem[indexString]] + ': '
                                + data[datasetString][0][dataString][tooltipItem[indexString]] + '%';
                        },
                    },
                },
            });
        }
    }
</script>
