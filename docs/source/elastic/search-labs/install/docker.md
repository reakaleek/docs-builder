---
title: "Docker"
---

The most convenient option for local development and testing with Elasticsearch is to use the official Docker image. To begin, make sure you have Docker installed on your system.

- [Install Docker](https://docs.docker.com/get-docker/)

## Running in Single Node Mode

For the purposes of this tutorial, a single node deployment of Elasticsearch is sufficient. The command below starts the service in a Docker container:

```bash
docker run -p 127.0.0.1:9200:9200 -d --name elasticsearch \
  -e "discovery.type=single-node" \
  -e "xpack.security.enabled=false" \
  -e "xpack.license.self_generated.type=trial" \
  -v "elasticsearch-data:/usr/share/elasticsearch/data" \
  docker.elastic.co/elasticsearch/elasticsearch:8.15.0
```

You may want to change the version Elasticsearch number in the command above for the latest version available.

A few seconds after running the command, the Elasticsearch service should be running on http://localhost:9200. Open this link on your browser to confirm that the service is up and running.

Note that the above command starts the service with authentication and encryption disabled, which means that anyone who connects to the service will be given access. While this is convenient for experimenting and learning Elasticsearch, **<u>you should never run the service in this way in production, or in a computer that is directly connected to the Internet</u>**.

The service is started with a trial license. The trial license enables all features of Elasticsearch, including RRF and ML Inference, for a trial period of 30 days. After the trial period expires, the license is downgraded to a basic license, which is free forever. If you prefer to skip the trial and use the basic license, set the value of the `xpack.license.self_generated.type` variable to `basic` instead. For a detailed feature comparison between the different licenses, refer to https://www.elastic.co/subscriptions.

## Self-Hosted Production Deployments

If you are interested in a self-hosted production deployment of Elasticsearch, refer to the following links:

- [Install Elasticsearch on Docker](https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html)
- [Elastic Cloud on Kubernetes](https://www.elastic.co/guide/en/cloud-on-k8s/current/index.html)