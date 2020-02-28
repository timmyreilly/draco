# Why did we create Draco?

## Background

Over the last decade, the way that software companies ([indepependent solution vendors or ISVs](https://blogs.partner.microsoft.com/mpn-canada/whats-isv-faq-microsoft-partners-independent-software-vendors-app-builders/)) deliver software to their end users has undergone a dramatic shift away from traditional, boxed software to [software as a service](https://azure.microsoft.com/en-us/overview/what-is-saas/) or, as its commonly referred to, SaaS. In this model, software is delivered not only to the desktop but also as a globally-available service that can be consumed by anyone, anywhere, on any device through a unified, fully-connected user experience.

While the engineering effort required to transform an existing traditional software product into a SaaS-enabled one can be monumental, the real challenge comes in the way that such software is sold. Whereas traditionally, customers purchase software as a [capital expenditure (CapEx)](https://en.wikipedia.org/wiki/Capital_expenditure), in a SaaS-enabled world, customers purchase software as an [operational expenditure (OpEx)](https://en.wikipedia.org/wiki/Operating_expense) paying only for what they consume on a monthly or annual basis.

Nowhere is this shift more apparent than here at Microsoft. Only a few years ago, using any of the applications that make up the [Microsoft Office](https://products.office.com/en-us/home) suite meant installing and configuring software on a potentially large number of individual machines. When you layer on the complexities of operating in a modern enterprise environment – licensing, configuration, security, and a wide array of other IT challenges – managing these installations becomes a massive job in and of itself. Today, however, installing Office directly onto the desktop is only one option that Microsoft makes available to its customers. Indeed, [Office 365](https://products.office.com/en-us/home) has made it possible not only to use the complete suite of Office applications anywhere – on the desktop, on the mobile device, or through the browser – but, it's also connected all these experiences through an always-on, globally-available, fully-managed platform backed by the tremendous computing power of [Microsoft Azure](https://azure.microsoft.com/). This hasn't been an easy journey but, in retrospect, most agree that it's been a worthwhile one.

Coming full circle, today, [Microsoft partners with some of the largest ISVs in the world across every imaginable industry](https://azure.microsoft.com/en-us/case-studies/) – from healthcare to government to manufacturing and beyond – with a core focus on helping them realize the promise of SaaS as part of their own product transformation. Through our own journey, we are in a unique position not only to help customers overcome the engineering challenges inherent in creating SaaS offerings but also to apply lessons learned through our own challenges in helping customers shift from a CapEx sales model to an OpEx one.

> In the spirit of complete transparency, Draco is also designed to solve one of Microsoft's largest problems – creating a reusable "on-ramp" for bringing its traditionally on-premises customers to the cloud. Our hope is that Draco provides at least part of the solution through not only a collection of reusable code assets (through this project) but also as an engagement model across our partner-facing business and technical organizations to customize and build on Draco to meet specific customer needs.

## The reality

While the shift to SaaS brings with it a broad spectrum of ubiquitous advantages, in the real world, there are a great many applications that millions of enterprise users depend on every day that, practically, will never be able to be delivered *exclusively* as a service. Looking at healthcare, for instance, there are some software solutions that will always need to be deployed locally at the point of care especially around the capture and secure processing of medical images. The same is true for retail where there will always need to be certain software available at the point of sale regardless of internet connectivity. Beyond the obvious implications, the challenges in taking an existing, on-premises solution and transforming it wholesale to a SaaS offering are monumental, especially when dealing with a large, globally-distributed client base that, often, has no concept of consuming software as a service. At the same time, the rise of enterprise SaaS is placing overwhelming pressure on ISVs to deliver updates more frequently, enable experiences across devices, and reduce crushing IT management burden.

## The solution

What's needed here is a bridge between the traditional, boxed, CapEx software model and SaaS that enables existing users to continue using the software they know and love today but, at the same time, take advantage of the flexibility and globally-connected experience that SaaS has to offer. 

Draco is that bridge.

## Our unique value proposition

Through **[extensions](/doc/architecture/definitions.md#extension)**, which Draco defines simply as *uniquely identifiable, independently versioned, self-contained application features*, ISVs with large, traditional, on-premises install bases can bring their applications into a SaaS-enabled world while creating a foundation for more upsell opportunities, a thriving partner ecosystem, and a mechanism for gaining rich, detailed insights into how customers are actually using their applications.

The [novel architectural features](/doc/architecture/overview.md) that lie at the heart of Draco – from **[execution models](/doc/architecture/execution-models.md)** to **[execution objects](/doc/architecture/execution-objects.md)** to **[extension services](/doc/architecture/extension-services.md)** – were designed not in a vacuum but based on decades of combined, real-world experience that Microsoft has working with a massive network of partners, from the largest Fortune 50 corporations to the smallest, born-in-the-cloud startups, spanning every imaginable industry and geography. 

### Quick and seamless updates

As your customer base grows, pushing software updates can become a complex and arduous task, especially when your customers are on-premises. Cloud-based extensions can be pushed to millions of users in seconds allowing you to react much more quickly to constantly changing customer needs.

### Partner ecosystem foundation
Draco lays a solid foundation for a rich partner ecosystem that revolves around your existing product offerings.

### More upsell opportunities
Upselling can be a major challenge especially for traditionally on-premises product offerings. Draco’s AI-enabled intelligent catalog makes it easy to drive targeted, personalized extension marketing campaigns.

### Better understand customer behavior and discover actionable insights
Draco generates rich, detailed usage information that can be used to drive analytics and business intelligence enabling you to discover actionable insights into how your customers are using your software.

### Low-risk cloud spend
The infrastructure required to host Draco is minimal requiring only a small up-front investment.

The real revenue is driven by the extensions themselves. Extensions can be scaled out then back in as needed based on customer demand. Since extensions can be provisioned “just-in-time”, cloud spend can be easily and safely controlled.


