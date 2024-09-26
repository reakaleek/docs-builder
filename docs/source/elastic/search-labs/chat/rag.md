---
title: "Project Setup"
---

In this section you will install, configure and run the Search Labs [Chatbot](https://github.com/elastic/elasticsearch-labs/tree/main/example-apps/chatbot-rag-app) example application.

## Run Elasticsearch

Before you begin, you should set up an Elasticsearch instance that you can use with this project. The easiest and most convenient path is to create a Elastic Cloud account, which includes a free trial period. But you can also opt to run Elasticsearch locally if that works better for you. See the [Install Elasticsearch](../install.md) section for installation instructions covering these two options.

## Obtain the Source code

The next step is for you to download the source code for the chatbot application. The following sub-sections describe a few ways you can do this. You can find the method that works best for you.

### Clone the Search Labs Repository

The Chatbot example is included in the Search Labs source code repository. You can clone this repository with the following command:

```bash
git clone https://github.com/elastic/elasticsearch-labs
cd elasticsearch-labs/example-apps/chatbot-rag-app
```

The Chatbot example is located in the **example-apps/chatbot-rag-app** subdirectory.

### Download a Tarball

On macOS or Linux, you can download a tarball of the Search Labs source code repository, and then extract this application from it. The command to do this is:

```bash
curl https://codeload.github.com/elastic/elasticsearch-labs/tar.gz/main | \
tar -xz --strip=2 elasticsearch-labs-main/example-apps/chatbot-rag-app
cd chatbot-rag-app
```

### Download a Zip File

If you are more comfortable working with zip files, you can download the Search Labs repository [here]( https://codeload.github.com/elastic/elasticsearch-labs/zip/main).

Once you have the zip file, use your favorite unzip tool to extract the **elasticsearch-labs-main/example-apps/chatbot-rag-app** to a location on your disk.
