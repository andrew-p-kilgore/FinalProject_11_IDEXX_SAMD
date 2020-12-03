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
        <requests-by-endpoint v-if="loaded" :data="chartData" :title="title" />
        <requests-by-endpoint2 v-if="loaded" :data="chartData2" :title="title" />
    </v-container>
</template>



<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import RequestsByEndpoint from '@/components/RequestsByEndpoint.vue';
    import RequestsByEndpoint2 from '@/components/RequestsByEndpoint2.vue';
    @Component({
        components: { RequestsByEndpoint, RequestsByEndpoint2 },
    })
    export default class RequestsByEndpointView extends Vue {
        private loaded: boolean = false;
        private showError: boolean = false;
        private errorMessage: string = 'Error while loading.';
        private chartData: object = {};
        private chartData2: object = {};
        private title: string = 'Number of requests by endpoint';
        private async mounted() {
            this.loaded = false;
            try {
                const response = await this.$axios.get('api/RequestsByEndpoint?name="All"');
                const response2 = await this.$axios.get('api/RequestsByEndpoint2?name="All"');
                this.chartData = response.data.chartData;
                this.chartData2 = response2.data.chartData;
                this.loaded = true;
            } catch (e) {
                this.showError = true;
                this.errorMessage = `Error while loading: ${e.message}.`;
            }
        }

        private async getData(group: any) {
            this.loaded = false;
            try {
                const response = await this.$axios.get('api/RequestsByEndpoint?name="' + group.item + '"');
                const response2 = await this.$axios.get('api/RequestsByEndpoint2?name="' + group.item + '"');
                this.chartData = response.data.chartData;
                this.chartData2 = response2.data.chartData;
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