<template>
    <v-container fluid>
        <template>
            <v-container fluid>
                <v-row align="center">
                    <v-col cols="3">
                        <v-subheader>
                            Select what to display:
                        </v-subheader>
                    </v-col>

                    <v-col cols="3">
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

                    <v-col cols ="3">
                        <v-subheader>
                            Blank charts with a title indicate that the partner has made no requests over the time period
                        </v-subheader>
                    </v-col>
                </v-row>
            </v-container>
        </template>
        <response-types v-if="loaded" :data="chartData" v-bind:title="title" style="float: left; width: 45%;" />
        <response-types v-if="loaded" :data="chartData2" :title="title2" style="float: right; width: 45%;" />
    </v-container>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import ResponseTypes from '@/components/ResponseTypes.vue';

    @Component({
        components: { ResponseTypes },
    })
    export default class ResponseTypesView extends Vue {
        private loaded: boolean = false;
        private showError: boolean = false;
        private errorMessage: string = 'Error while loading.';
        private chartData: object = {};
        private chartData2: object = {};
        private title: string = 'Response Types in Last 24 Hours';
        private title2: string = 'Response Types in Last 7 Days';

        // Webpage defaultly starts on the "All" option
        private async mounted() {
            this.loaded = false;
            try {
                const response = await this.$axios.get('api/ResponseTypes?name="All"');
                const response2 = await this.$axios.get('api/ResponseTypesWeek?name="All"');
                this.chartData = response.data.chartData;
                this.chartData2 = response2.data.chartData;
                this.loaded = true;
            } catch (e) {
                this.showError = true;
                this.errorMessage = `Error while loading: ${e.message}.`;
            }
        }

        // This is called whenever one the dropdown menu options is selected
        // Gets data for a specfied partner group
        private async getData(group: any) {
            this.loaded = false;
            try {
                const response = await this.$axios.get('api/ResponseTypes?name="' + group.item + '"');
                const response2 = await this.$axios.get('api/ResponseTypesWeek?name="' + group.item + '"');
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
