# Your first Value Object

<card-summary>
Create and use your first Value Object
</card-summary>

[Install](Installation.md) the package, and create a new Value Object:

<tabs>
    <tab title="Generics">
<code-block lang="c#">
    <![CDATA[
        [ValueObject<int>] 
        public partial struct CustomerId { }
    ]]>
</code-block>
    </tab>
    <tab title="Non-generics" >
        <code-block lang="c#">
            <![CDATA[
            [ValueObject(typeof(int))] 
            public partial struct CustomerId { }
            ]]>
        </code-block>
    </tab>
</tabs>

<note>
partial is required as the code generator augments this type by creating another partial class
</note>

Create a new instance by using the `From` method:

```c#
var customerId = CustomerId.From(42);
```

If you try to use a constructor, then the [analyzer rules](Analyzer-Rules.md) will catch this and stop you.

You can now be more explicit in your methods with signatures such as:

```c#
public void HandlePayment(
    CustomerId customerId, 
    AccountId accountId, 
    PaymentAmount paymentAmount)
```


