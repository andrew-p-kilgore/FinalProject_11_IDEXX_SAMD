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
                                  single-line></v-select>
                    </v-col>
                </v-row>
            </v-container>
        </template>
        <test-graph-2 v-if="loaded" :data="chartData" />
        <test-graph-3 v-if="loaded" :data="chartData2" />
        <test-graph-4 v-if="loaded" :data="chartData3" />
    </v-container>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import TestGraph2 from '@/components/TestGraph2.vue';
    import TestGraph3 from '@/components/TestGraph3.vue';
    import TestGraph4 from '@/components/TestGraph4.vue';
    @Component({
        components: { TestGraph2, TestGraph3, TestGraph4 },
    })
    export default class TestGraph2View extends Vue {
        private loaded: boolean = false;
        private showError: boolean = false;
        private errorMessage: string = 'Error while loading.';
        private chartData: object = {};
        private chartData2: object = {};
        private chartData3: object = {};
        private async mounted() {
            this.loaded = false;
            try {
                const response = await this.$axios.get('api/TestGraph2');
                const response2 = await this.$axios.get('api/TestGraph3');
                const response3 = await this.$axios.get('api/TestGraph4');
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
                    //{ item: 'Regression Cornerstone Group', abbr: 'RCG' },
                    //{ item: 'Communicator Test', abbr: 'CT' },
                    //{ item: 'QA CIS BATCH TEST', abbr: 'QaCBT' },
                    //{ item: 'Other Partners', abbr: 'Other' },
                ],
            };
        }
    }
</script>