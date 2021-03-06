using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;



namespace VirtoCommerce.SwaggerApiClient.Model {

  /// <summary>
  /// 
  /// </summary>
  [DataContract]
  public class VirtoCommerceCartModuleWebModelPaymentMethod {
    
    /// <summary>
    /// Gets or sets the value of payment gateway code
    /// </summary>
    /// <value>Gets or sets the value of payment gateway code</value>
    [DataMember(Name="gatewayCode", EmitDefaultValue=false)]
    public string GatewayCode { get; set; }

    
    /// <summary>
    /// Gets or sets the value of payment method name
    /// </summary>
    /// <value>Gets or sets the value of payment method name</value>
    [DataMember(Name="name", EmitDefaultValue=false)]
    public string Name { get; set; }

    
    /// <summary>
    /// Gets or sets the value of payment method logo absolute URL
    /// </summary>
    /// <value>Gets or sets the value of payment method logo absolute URL</value>
    [DataMember(Name="iconUrl", EmitDefaultValue=false)]
    public string IconUrl { get; set; }

    
    /// <summary>
    /// Gets or sets the value of payment method description
    /// </summary>
    /// <value>Gets or sets the value of payment method description</value>
    [DataMember(Name="description", EmitDefaultValue=false)]
    public string Description { get; set; }

    
    /// <summary>
    /// Gets or sets the value of payment method type
    /// </summary>
    /// <value>Gets or sets the value of payment method type</value>
    [DataMember(Name="type", EmitDefaultValue=false)]
    public string Type { get; set; }

    
    /// <summary>
    /// Gets or sets the value of payment method group type
    /// </summary>
    /// <value>Gets or sets the value of payment method group type</value>
    [DataMember(Name="group", EmitDefaultValue=false)]
    public string Group { get; set; }

    
    /// <summary>
    /// Gets or sets the value of payment method priority
    /// </summary>
    /// <value>Gets or sets the value of payment method priority</value>
    [DataMember(Name="priority", EmitDefaultValue=false)]
    public int? Priority { get; set; }

    

    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      var sb = new StringBuilder();
      sb.Append("class VirtoCommerceCartModuleWebModelPaymentMethod {\n");
      
      sb.Append("  GatewayCode: ").Append(GatewayCode).Append("\n");
      
      sb.Append("  Name: ").Append(Name).Append("\n");
      
      sb.Append("  IconUrl: ").Append(IconUrl).Append("\n");
      
      sb.Append("  Description: ").Append(Description).Append("\n");
      
      sb.Append("  Type: ").Append(Type).Append("\n");
      
      sb.Append("  Group: ").Append(Group).Append("\n");
      
      sb.Append("  Priority: ").Append(Priority).Append("\n");
      
      sb.Append("}\n");
      return sb.ToString();
    }

    /// <summary>
    /// Get the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson() {
      return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

}


}
