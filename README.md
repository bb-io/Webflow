# Blackbird.io Webflow

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Webflow is a cloud-based web design and development platform that enables users to create, customize, and manage responsive websites visually. It provides a wide range of features and tools for designers, developers, and businesses to build and maintain professional websites without needing to write code. Webflow allows users to publish websites directly, collaborate in real-time, and utilize a robust CMS to manage content. This makes it a versatile solution for individuals, entrepreneurs, and enterprises looking to establish and grow their online presence.

## Before connecting

1.  Ensure you have an active Webflow account. If not, create one here: [here](https://webflow.com/signup).
2.  Confirm that at least one site exists in your Webflow account.
3.  Open **Apps & Integrations** in Webflow and create a new application.

![create app](image/README/Create-app.png)

4.  Provide the application details (name, description and URL can be customized). In **Building blocks**, enable **Designer extension** and **Data client (REST API)**. 
Configure the OAuth redirect URL with: 'https://bridge.blackbird.io/api/AuthorizationCode'. Then select the required scopes.

![info input](image/README/Input-info.png)

![config input](image/README/Configuration-app.png)

5.  Click **Create app**. After creation, Webflow will generate a **Client ID** and **Client Secret**. Retain these credentials—they are required to connect Webflow to Blackbird.

## Connecting

1.  Navigate to Apps, and identify the **Webflow** app. You can use search to find it.
2.  Click _Add Connection_.
3.  Name your connection for future reference e.g. 'My organization'. Input the **Client ID** and **Client Secret** obtained from Webflow.
4.  Click _Authorize connection_.
5.  Follow the instructions that Webflow gives you.
6.  When you return to Blackbird, confirm that the connection has appeared and the status is _Connected_.

## Actions

### Collections

-   **Get/Create/Delete collection**

### Collection items
-   **Get collection item content as HTML** returns content of a specific collection item in HTML format.
-   **Publish collection item** publishes a specific collection item. This actions can only publish an item of the default locale, because of the Webflow API limitation.
-   **Update collection item content from HTML** updates content of a specific collection item from HTML file.

### Pages

- **Search pages** Search pages using filters
- **Get page content as HTML** Get the page content in HTML file
- **Update page content as HTML** Update page content using HTML file

### Components

- **List components** List all components for a site. Supports filtering by name, group, and read-only status.
- **Get component content as HTML** Get the component content in HTML file.
- **Update component content as HTML** Update component content using HTML file.

	
## Events

-   **On collection item changed** is triggered when a specific collection item is changed
-   **On collection item created** is triggered when a specific collection item is created
-   **On collection item deleted** is triggered when a specific collection item is deleted
-   **On collection item unpublished** is triggered when a specific collection item is unpublished
-   **On page created** is triggered when a specific page is created
-   **On page deleted** is triggered when a specific page is deleted
-   **On page metadata updated** is triggered when specific page metadata is updated
-   **On site published** is triggered when a specific site is published
-   **On page update** triggered when page update was made

## Example

![image](https://github.com/bb-io/Webflow/assets/137277669/77a4c14e-c505-4fb5-a813-3d46eb66ad8c)

## Missing features

Webflow is a huge app with a lot of features. If any of these features are particularly interesting to you, let us know!

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
