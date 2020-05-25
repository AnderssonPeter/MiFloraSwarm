import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { DataLine } from '../devices/device';
import { ChartType, ApexXAxis, ApexAxisChartSeries, ApexChart, ApexOptions, 
         ApexFill, ApexStroke, ApexDataLabels, ApexGrid, ApexYAxis, ApexResponsive } from 'ng-apexcharts';

@Component({
  selector: 'chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss']
})
export class ChartComponent implements OnInit, OnChanges {

  constructor() { }

  @Input()
  dataLines?: DataLine[] | DataLine;
  
  xaxis: ApexXAxis = {
    type: 'datetime',
    tooltip: {
      enabled: false
    },
    labels: {
      show: false
    }
  }
  yaxis: ApexYAxis = {
    show: false,
    min: 0
  }

  series?: ApexAxisChartSeries

  chart: ApexChart = {
    type: 'area',
    height: '25px',
    sparkline: {
      enabled: true
    },
    width: '100%'
  }

  grid: ApexGrid = {
    show: false,
    padding: {
      top: 5,
      bottom: 5
    }
  }

  dataLabels: ApexDataLabels = {
    enabled: false
  }

  fill: ApexFill = {
    type: 'gradient',
    colors: ['#FF0000', '#FFFF00'],
    gradient: {
      shadeIntensity: 1,
      inverseColors: false,
      opacityFrom: 1,
      opacityTo: 0,
      stops: [0, 90, 100]
    }
  }

  stroke: ApexStroke = {
    curve: "smooth",
    colors: ['#FF0000']
  }

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    //calculate series and xaxis
    if (this.dataLines instanceof DataLine) {
      this.dataLines = [this.dataLines];
    }
    this.series = this.dataLines?.map(dataLine => dataLine.toApexAxisChartSeries());
    
  }
}