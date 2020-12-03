export class ChartData {
    constructor(
        public labels: string[],
        public avg_duration: number[],
        public except_count: number[],
    ) { }
}
