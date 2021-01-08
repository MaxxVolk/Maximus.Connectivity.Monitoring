using Maximus.Library.SCOM.Editors;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maximus.Connectivity.UI.Control
{
  class IDs
  {
    #region Maximus.Connectivity.Monitoring.FullyQualifiedDomainName
    /// <summary>
    /// Fully Qualified Domain Name (Maximus.Connectivity.Monitoring.FullyQualifiedDomainName)
    /// </summary>
    public static Guid FullyQualifiedDomainNameClassId { get; } = new Guid("5338d09b-ec98-b619-6230-b29cfb49e428");
    public static class FullyQualifiedDomainNameClassProperties
    {
      /// <summary>
      /// Maximus.Connectivity.Monitoring.FullyQualifiedDomainName/FullyQualifiedDomainName
      /// </summary>
      public static Guid FullyQualifiedDomainNamePropertyId { get; } = new Guid("2830a410-9eef-6ecb-32d9-c491d6a985d2");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.FullyQualifiedDomainName/TargetIndex
      /// </summary>
      public static Guid TargetIndexPropertyId { get; } = new Guid("a40879d5-ce9f-4c72-57de-4ae179ba7707");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.FullyQualifiedDomainName/Description
      /// </summary>
      public static Guid DescriptionPropertyId { get; } = new Guid("9b405107-486b-a71c-e9fc-d28e6b1bce3a");
    }
    #endregion

    #region Maximus.Connectivity.Monitoring.Template
    /// <summary>
    /// Connectivity Test Template (Maximus.Connectivity.Monitoring.Template)
    /// </summary>
    public static Guid TestTemplateClassId { get; } = new Guid("a74c0df6-a44e-043b-0ada-2c933e951d39");
    public static class TestTemplateClassProperties
    {
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Template/Name
      /// </summary>
      public static Guid NamePropertyId { get; } = new Guid("72e39a17-ece9-7888-278a-e55d5ef1be62");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Template/Template
      /// </summary>
      public static Guid TemplatePropertyId { get; } = new Guid("fb8d471d-2d73-a95a-e75d-7c98f015b0ca");
    }
    #endregion

    #region Maximus.Connectivity.Monitoring.Test
    /// <summary>
    /// Connectivity Test Base (Abstract) (Maximus.Connectivity.Monitoring.Test)
    /// </summary>
    public static Guid TestBaseClassId { get; } = new Guid("c0acffc7-3bda-e2ce-796a-2d27fb49053a");
    public static class TestBaseClassProperties
    {
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test/TestId
      /// </summary>
      public static Guid TestIdPropertyId { get; } = new Guid("956f8798-ee96-efb8-de00-92bdd248b206");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test/TemplateReference
      /// </summary>
      public static Guid TemplateReferencePropertyId { get; } = new Guid("32821990-c7e3-8110-d910-6c8adb0a13bd");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test/IntervalSeconds
      /// </summary>
      public static Guid IntervalSecondsPropertyId { get; } = new Guid("57230b15-a16a-3502-1d0c-cc21418e8a12");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test/MatchCount
      /// </summary>
      public static Guid MatchCountPropertyId { get; } = new Guid("cfd08328-0661-bf94-70df-a2f02488f091");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test/SampleCount
      /// </summary>
      public static Guid SampleCountPropertyId { get; } = new Guid("676945e4-9bbc-e70e-c406-a2f135f4b9b6");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test/CollectPerformanceData
      /// </summary>
      public static Guid CollectPerformanceDataPropertyId { get; } = new Guid("b6527c3e-e088-20a0-a10e-812e057ce3ce");
    }
    #endregion
    
    #region Maximus.Connectivity.Monitoring.Test.Ping
    /// <summary>
    /// Connectivity Test - Ping (Maximus.Connectivity.Monitoring.Test.Ping)
    /// </summary>
    public static Guid PingTestClassId { get; } = new Guid("b1d0e508-aa36-1391-33e6-a9aff57bf8c0");
    public static class PingTestClassProperties
    {
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.Ping/MaxRTT
      /// </summary>
      public static Guid MaxRTTPropertyId { get; } = new Guid("165e26f1-d2c8-4706-174c-a32a257b1087");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.Ping/MaxTTL
      /// </summary>
      public static Guid MaxTTLPropertyId { get; } = new Guid("93f5f8a6-3c4a-affb-efde-6c5f4c2c4571");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.Ping/BufferSize
      /// </summary>
      public static Guid BufferSizePropertyId { get; } = new Guid("aa296a74-a41b-23dd-149f-e2eee4f4962d");
    }
    #endregion

    #region Maximus.Connectivity.Monitoring.Test.SSL
    /// <summary>
    /// Connectivity Test - Secure Socket Layer Connection (Maximus.Connectivity.Monitoring.Test.SSL)
    /// </summary>
    public static Guid SSLTestClassId { get; } = new Guid("080ff65c-aedb-01bb-2431-d51f4f1cf036");
    public static class SSLTestClassProperties
    {
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/Schema
      /// </summary>
      public static Guid SchemaPropertyId { get; } = new Guid("51ea4c5e-1839-5fa9-fada-c26a1f914128");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/Port
      /// </summary>
      public static Guid PortPropertyId { get; } = new Guid("5109ce5e-d54a-8a64-df6a-343007245875");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreRevocationCheck
      /// </summary>
      public static Guid IgnoreRevocationCheckPropertyId { get; } = new Guid("8d184ddb-0006-7362-a3ee-64e908105148");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/ApplicationPolicy
      /// </summary>
      public static Guid ApplicationPolicyPropertyId { get; } = new Guid("310d74f0-f151-b405-e10e-00f0462e4246");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/CertificatePolicy
      /// </summary>
      public static Guid CertificatePolicyPropertyId { get; } = new Guid("2884dc79-f40c-0e13-d571-50cc40bad1dd");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/AllowUnknownCertificateAuthority
      /// </summary>
      public static Guid AllowUnknownCertificateAuthorityPropertyId { get; } = new Guid("cf684511-e9fa-a7c3-31eb-23e5ca962088");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreCertificateAuthorityRevocationUnknown
      /// </summary>
      public static Guid IgnoreCertificateAuthorityRevocationUnknownPropertyId { get; } = new Guid("25cf1fd0-5c37-41ed-18d9-6961f97c2b59");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreCtlNotTimeValid
      /// </summary>
      public static Guid IgnoreCtlNotTimeValidPropertyId { get; } = new Guid("85d6372c-d6e5-af15-c953-e366c7654460");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreCtlSignerRevocationUnknown
      /// </summary>
      public static Guid IgnoreCtlSignerRevocationUnknownPropertyId { get; } = new Guid("0432d4c4-9e34-2133-fb77-e579da042d87");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreEndRevocationUnknown
      /// </summary>
      public static Guid IgnoreEndRevocationUnknownPropertyId { get; } = new Guid("58f0c59f-8baf-045a-464c-c46fbd612d28");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreInvalidBasicConstraints
      /// </summary>
      public static Guid IgnoreInvalidBasicConstraintsPropertyId { get; } = new Guid("d0424e30-50fd-37c4-77b2-ed520d557779");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreInvalidName
      /// </summary>
      public static Guid IgnoreInvalidNamePropertyId { get; } = new Guid("506a8980-886a-e469-71a2-8d2bfe4c240c");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreInvalidPolicy
      /// </summary>
      public static Guid IgnoreInvalidPolicyPropertyId { get; } = new Guid("72cf2937-5051-7a93-10ad-ea085ab7aa43");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreNotTimeNested
      /// </summary>
      public static Guid IgnoreNotTimeNestedPropertyId { get; } = new Guid("8851f19a-cd37-6192-271e-6f5bd709b478");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreNotTimeValid
      /// </summary>
      public static Guid IgnoreNotTimeValidPropertyId { get; } = new Guid("1a37f8dc-725b-9c04-3f78-89f4215da21f");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreRootRevocationUnknown
      /// </summary>
      public static Guid IgnoreRootRevocationUnknownPropertyId { get; } = new Guid("ece305f3-e70d-cb16-b328-5000f8678494");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.SSL/IgnoreWrongUsage
      /// </summary>
      public static Guid IgnoreWrongUsagePropertyId { get; } = new Guid("6542ccf6-6cd5-17bf-bca6-e10bad1a3898");
    }
    #endregion

    #region Maximus.Connectivity.Monitoring.Test.HTTP
    /// <summary>
    /// Connectivity Test - HTTP Probe (Maximus.Connectivity.Monitoring.Test.HTTP)
    /// </summary>
    public static Guid HTTPTestClassId { get; } = new Guid("c021bf1f-e826-32d4-78bc-684344b2baec");
    public static class HTTPTestClassProperties
    {
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/Schema
      /// </summary>
      public static Guid SchemaPropertyId { get; } = new Guid("a5cf3121-1f3b-abc4-aed1-7d4a226c4bb5");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/Path
      /// </summary>
      public static Guid PathPropertyId { get; } = new Guid("ffc2e2cc-40e3-5fcc-a2c1-a65b0828fb90");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/AllowRedirect
      /// </summary>
      public static Guid AllowRedirectPropertyId { get; } = new Guid("3d140974-655a-d42c-5879-6872f08d579c");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/HTTPMethod
      /// </summary>
      public static Guid HTTPMethodPropertyId { get; } = new Guid("47bdec74-2954-97d2-5fba-3bdf18a22346");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/RequestBody
      /// </summary>
      public static Guid RequestBodyPropertyId { get; } = new Guid("a1db7d51-9d07-21ef-2ff3-7b70e1ca4bb3");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/RegularExpression
      /// </summary>
      public static Guid RegularExpressionPropertyId { get; } = new Guid("9f56e01f-0a58-2933-1e1b-6b4229e7cce3");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/ExpressionIsNegative
      /// </summary>
      public static Guid ExpressionIsNegativePropertyId { get; } = new Guid("8391e27e-a105-8d57-84bc-d41f5477d161");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/ExtraHeaders
      /// </summary>
      public static Guid ExtraHeadersPropertyId { get; } = new Guid("f3c96b65-c9a4-77ca-af87-6a95ef8d3e01");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/Host
      /// </summary>
      public static Guid HostPropertyId { get; } = new Guid("4cdb3dc5-62bc-2758-422a-9feaf1519ada");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/Accept
      /// </summary>
      public static Guid AcceptPropertyId { get; } = new Guid("428ba717-03e0-5060-89d2-bc236e5ae2b0");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/ContentType
      /// </summary>
      public static Guid ContentTypePropertyId { get; } = new Guid("50bf244e-9224-73b7-c448-97f700d2d36a");
      /// <summary>
      /// Maximus.Connectivity.Monitoring.Test.HTTP/UserAgent
      /// </summary>
      public static Guid UserAgentPropertyId { get; } = new Guid("56d17dd3-c165-a3f5-5a9b-61772c36cf9d");
    }
    #endregion
    
  }
}
