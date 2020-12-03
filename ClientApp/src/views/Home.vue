
<template>
    <v-container fluid>

        <v-slide-y-transition mode="out-in">
            <v-row>
                <v-col>
                    <h1>Basic API Call</h1>
                    <p>This displays data requested from API</p>

                    <span>Message: {{ requestData }}</span>
                </v-col>
            </v-row>
        </v-slide-y-transition>

    </v-container>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { Data } from '../models/Data';

    @Component({})
    export default class Home extends Vue {
        private loading: boolean = true;
        private showError: boolean = false;
        private errorMessage: string = 'Error while loading.';
        private requestData: Data[] = [];

        private async created() {
            await this.fetchData();
        }

        private async fetchData() {
            try {
                const response = await this.$axios.get('api/ReqPercent');
                this.requestData = response.data;
            } catch (e) {
                this.showError = true;
                this.errorMessage = `Error while loading: ${e.message}.`;
            }
            this.loading = false;
        }
    }
</script>
