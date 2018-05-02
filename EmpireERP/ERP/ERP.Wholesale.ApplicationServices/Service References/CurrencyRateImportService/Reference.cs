﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.Wholesale.ApplicationServices.CurrencyRateImportService {
    using System.Data;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://web.cbr.ru/", ConfigurationName="CurrencyRateImportService.DailyInfoSoap")]
    public interface DailyInfoSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/SaldoXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode SaldoXML(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/mrrf7D", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet mrrf7D(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/mrrf7DXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode mrrf7DXML(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/mrrf", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet mrrf(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/mrrfXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode mrrfXML(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/Saldo", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet Saldo(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/NewsInfoXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode NewsInfoXML(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/OmodInfoXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode OmodInfoXML();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/XVol", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet XVol(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/XVolXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode XVolXML(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/MainInfoXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode MainInfoXML();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/AllDataInfoXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode AllDataInfoXML();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/NewsInfo", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet NewsInfo(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/SwapDynamicXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode SwapDynamicXML(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/SwapDynamic", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet SwapDynamic(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/DepoDynamicXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode DepoDynamicXML(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/DepoDynamic", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet DepoDynamic(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/OstatDynamicXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode OstatDynamicXML(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/OstatDynamic", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet OstatDynamic(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/DragMetDynamicXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode DragMetDynamicXML(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/DragMetDynamic", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet DragMetDynamic(System.DateTime fromDate, System.DateTime ToDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/GetLatestDateTime", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.DateTime GetLatestDateTime();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/GetLatestDate", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string GetLatestDate();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/GetLatestDateTimeSeld", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.DateTime GetLatestDateTimeSeld();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/GetLatestDateSeld", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string GetLatestDateSeld();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/EnumValutesXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode EnumValutesXML(bool Seld);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/EnumValutes", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet EnumValutes(bool Seld);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/GetCursDynamicXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode GetCursDynamicXML(System.DateTime FromDate, System.DateTime ToDate, string ValutaCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/GetCursDynamic", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet GetCursDynamic(System.DateTime FromDate, System.DateTime ToDate, string ValutaCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/GetCursOnDateXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode GetCursOnDateXML(System.DateTime On_date);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/GetSeldCursOnDateXML", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Xml.XmlNode GetSeldCursOnDateXML(System.DateTime On_date);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/GetSeldCursOnDate", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet GetSeldCursOnDate(System.DateTime On_date);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://web.cbr.ru/GetCursOnDate", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataSet GetCursOnDate(System.DateTime On_date);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface DailyInfoSoapChannel : ERP.Wholesale.ApplicationServices.CurrencyRateImportService.DailyInfoSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DailyInfoSoapClient : System.ServiceModel.ClientBase<ERP.Wholesale.ApplicationServices.CurrencyRateImportService.DailyInfoSoap>, ERP.Wholesale.ApplicationServices.CurrencyRateImportService.DailyInfoSoap {
        
        public DailyInfoSoapClient() {
        }
        
        public DailyInfoSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DailyInfoSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DailyInfoSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DailyInfoSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Xml.XmlNode SaldoXML(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.SaldoXML(fromDate, ToDate);
        }
        
        public System.Data.DataSet mrrf7D(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.mrrf7D(fromDate, ToDate);
        }
        
        public System.Xml.XmlNode mrrf7DXML(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.mrrf7DXML(fromDate, ToDate);
        }
        
        public System.Data.DataSet mrrf(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.mrrf(fromDate, ToDate);
        }
        
        public System.Xml.XmlNode mrrfXML(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.mrrfXML(fromDate, ToDate);
        }
        
        public System.Data.DataSet Saldo(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.Saldo(fromDate, ToDate);
        }
        
        public System.Xml.XmlNode NewsInfoXML(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.NewsInfoXML(fromDate, ToDate);
        }
        
        public System.Xml.XmlNode OmodInfoXML() {
            return base.Channel.OmodInfoXML();
        }
        
        public System.Data.DataSet XVol(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.XVol(fromDate, ToDate);
        }
        
        public System.Xml.XmlNode XVolXML(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.XVolXML(fromDate, ToDate);
        }
        
        public System.Xml.XmlNode MainInfoXML() {
            return base.Channel.MainInfoXML();
        }
        
        public System.Xml.XmlNode AllDataInfoXML() {
            return base.Channel.AllDataInfoXML();
        }
        
        public System.Data.DataSet NewsInfo(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.NewsInfo(fromDate, ToDate);
        }
        
        public System.Xml.XmlNode SwapDynamicXML(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.SwapDynamicXML(fromDate, ToDate);
        }
        
        public System.Data.DataSet SwapDynamic(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.SwapDynamic(fromDate, ToDate);
        }
        
        public System.Xml.XmlNode DepoDynamicXML(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.DepoDynamicXML(fromDate, ToDate);
        }
        
        public System.Data.DataSet DepoDynamic(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.DepoDynamic(fromDate, ToDate);
        }
        
        public System.Xml.XmlNode OstatDynamicXML(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.OstatDynamicXML(fromDate, ToDate);
        }
        
        public System.Data.DataSet OstatDynamic(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.OstatDynamic(fromDate, ToDate);
        }
        
        public System.Xml.XmlNode DragMetDynamicXML(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.DragMetDynamicXML(fromDate, ToDate);
        }
        
        public System.Data.DataSet DragMetDynamic(System.DateTime fromDate, System.DateTime ToDate) {
            return base.Channel.DragMetDynamic(fromDate, ToDate);
        }
        
        public System.DateTime GetLatestDateTime() {
            return base.Channel.GetLatestDateTime();
        }
        
        public string GetLatestDate() {
            return base.Channel.GetLatestDate();
        }
        
        public System.DateTime GetLatestDateTimeSeld() {
            return base.Channel.GetLatestDateTimeSeld();
        }
        
        public string GetLatestDateSeld() {
            return base.Channel.GetLatestDateSeld();
        }
        
        public System.Xml.XmlNode EnumValutesXML(bool Seld) {
            return base.Channel.EnumValutesXML(Seld);
        }
        
        public System.Data.DataSet EnumValutes(bool Seld) {
            return base.Channel.EnumValutes(Seld);
        }
        
        public System.Xml.XmlNode GetCursDynamicXML(System.DateTime FromDate, System.DateTime ToDate, string ValutaCode) {
            return base.Channel.GetCursDynamicXML(FromDate, ToDate, ValutaCode);
        }
        
        public System.Data.DataSet GetCursDynamic(System.DateTime FromDate, System.DateTime ToDate, string ValutaCode) {
            return base.Channel.GetCursDynamic(FromDate, ToDate, ValutaCode);
        }
        
        public System.Xml.XmlNode GetCursOnDateXML(System.DateTime On_date) {
            return base.Channel.GetCursOnDateXML(On_date);
        }
        
        public System.Xml.XmlNode GetSeldCursOnDateXML(System.DateTime On_date) {
            return base.Channel.GetSeldCursOnDateXML(On_date);
        }
        
        public System.Data.DataSet GetSeldCursOnDate(System.DateTime On_date) {
            return base.Channel.GetSeldCursOnDate(On_date);
        }
        
        public System.Data.DataSet GetCursOnDate(System.DateTime On_date) {
            return base.Channel.GetCursOnDate(On_date);
        }
    }
}
