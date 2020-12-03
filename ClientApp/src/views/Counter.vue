<template>
    <v-container fluid>
        <template>
            <v-container fluid>
                <v-row align="center">
                    <v-col cols="2">
                        <v-subheader>
                            Select what to display:
                        </v-subheader>
                    </v-col>

                    <v-col cols="2">
                        <v-select v-model="select"
                                  :hint="`Showing data for: ${select.item}`"
                                  :items="items"
                                  item-text="item"
                                  item-value="abbr"
                                  label="Select"
                                  dense
                                  persistent-hint
                                  return-object
                                  single-line
                                  v-on:change="getData"></v-select>
                    </v-col>
                </v-row>
            </v-container>
        </template>
        <test-comp v-if="loaded" :data="chartData" />
        <request-graph-2 v-if="loaded" :data="chartData2" />
        <request-graph-3 v-if="loaded" :data="chartData3" />
    </v-container>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import TestComp from '@/components/TestComp.vue';
    import RequestGraph2 from '@/components/RequestGraph2.vue';
    import RequestGraph3 from '@/components/RequestGraph3.vue';
    @Component({
        components: { TestComp, RequestGraph2, RequestGraph3 },
    })
    export default class CounterView extends Vue {
        private loaded: boolean = false;
        private showError: boolean = false;
        private errorMessage: string = 'Error while loading.';
        private chartData: object = {};
        private chartData2: object = {};
        private chartData3: object = {};
        private async mounted() {
            this.loaded = false;
            try {
                const response = await this.$axios.get('api/AppInsights?name="All"');
                const response2 = await this.$axios.get('api/RequestGraph2?name="All"');
                const response3 = await this.$axios.get('api/RequestGraph3?name="All"');
                this.chartData = response.data.chartData;
                this.chartData2 = response2.data.chartData;
                this.chartData3 = response3.data.chartData;
                this.loaded = true;
            } catch (e) {
                this.showError = true;
                this.errorMessage = `Error while loading: ${e.message}.`;
            }
        }

        private async getData(group: any) {
            this.loaded = false;
            try {
                const response = await this.$axios.get('api/AppInsights?name="' + group.item + '"');
                const response2 = await this.$axios.get('api/RequestGraph2?name="' + group.item + '"');
                const response3 = await this.$axios.get('api/RequestGraph3?name="' + group.item + '"');
                this.chartData = response.data.chartData;
                this.chartData2 = response2.data.chartData;
                this.chartData3 = response3.data.chartData;
                this.loaded = true;
            } catch (e) {
                this.showError = true;
                this.errorMessage = `Error while loading: ${e.message}.`;
            }
        }

        private data() {
            return {
                select: { item: 'All', abbr: 'All' },
                items: [
                    { item: 'All', abbr: 'All' },
                    { item: 'Regression Cornerstone Group', abbr: 'RCG' },
                    { item: 'Communicator Test', abbr: 'CT' },
                    { item: 'QA CIS BATCH TEST', abbr: 'QaCBT' },
                    { item: 'Other Partners', abbr: 'Other' },
                ],
            };
        }
    }
</script>