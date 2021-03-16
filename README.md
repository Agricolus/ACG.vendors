# FIWARE AgriContractorsGateway

[![FIWARE Banner](https://fiware.github.io/tutorials.Context-Providers/img/fiware.png)](https://www.fiware.org/developers)

### A first FIWARE Domain Application: AgriContractorsGateway

[![FIWARE Core Context Management](https://nexus.lab.fiware.org/repository/raw/public/badges/chapters/core.svg)](https://github.com/FIWARE/catalogue/blob/master/core/README.md)
[![License badge](https://img.shields.io/github/license/FIWARE/context.Orion-LD.svg)](https://opensource.org/licenses/AGPL-3.0)
[![Support badge](https://nexus.lab.fiware.org/repository/raw/public/badges/stackoverflow/fiware.svg)](https://stackoverflow.com/questions/tagged/fiware)
[![NGSI-LD badge](https://img.shields.io/badge/NGSI-LD-red.svg)](https://www.etsi.org/deliver/etsi_gs/CIM/001_099/009/01.02.01_60/gs_CIM009v010201p.pdf)
[![Documentation](https://img.shields.io/readthedocs/fiware-tutorials.svg)](https://fiware-tutorials.rtfd.io)

## Table of Contents

* [Introduction](#introduction)
* [Architecture](#architecture)
* [Functionalities](#functionalities)
* [APIs](#apis)
* [Frontend](#frontend)
* [License](#license)

## Introduction

The main purpose of this project is to provide an idea of a [FIWARE](https://www.fiware.org) Domain Specific Enabler related to the [FIWARE AgriFood Reference Architecture](https://www.fiware.org/community/smart-agrifood). This DSE enables a standardized connection to many agricolture machinery in the market relying on [Orion Context Broker](https://fiware-orion.readthedocs.io/en/master) technology and [NGSI-LD](https://fiware-datamodels.readthedocs.io/en/latest/ngsi-ld_faq) standard. 

This product has some major interests: 

* Providing a standardized interface to several producers that can be used by AG platforms and other AG related products
* Providing a standalone platform that can be used to visualize data coming from different machines
* Providing a product that can be used event by agricolutral service providers (contractors)

In a first market analysis this kind of product is missing and, moreover, is more than welcomed by the public administration and by platform producers to simplify the access to this kind of data. 

This idea starts from [Agricolus](https://www.agricolus.com/en), in the [IOF2020](https://www.iof2020.eu) ecosystem and recognized as a common benefit to run a joint effort to build this GE. The company recognizes FIWARE as the natural architecture to choose for this kind of development. 

This Gateway is both a standalone application and a data gateway for machinery data: 

* The **standalone application**: is provided to deploy a monitoring and data collection system for contractors, association of farmers and other organizations that needs to collect, visualize and deliver these data to third parties. With this DSE they will be able to setup an environment capable of implement several communication standards for several hardware producers with a single point of access
* The **data gateway**: each organization will be able to deliver, upon authorization of each entity, these dynamic data to third parties, such as AG platforms and mobile apps, to enable quickly and with a reduced cost new services and functionalities

## Architecture
![Backend services](https://user-images.githubusercontent.com/7735943/111140021-23ee6200-8582-11eb-9ca8-f0abb17aea2c.jpg)

* **Vendors' ADAP SDK Plugins**: 
   * Will interact with different producers ecosystems and cloud applications. Each producer may allow the access to a cloud repository or directly to the data through theirs proprietary ADAPT representations. This ACG.Vendors module will host the vendors' ADAP Plugins and use them to convert proprietary representations to generic ADAPT representation.
* **ADAPT - ISOXML Converter**:
   * Will convert the generic ADAPT representation to ISOXML format.
* **ISOXML - IoT Agent**:
   * Will convert the generic ISOXML representation to context broker entities.
* **FIWARE ContextBroker**:
   * Will dispatch machines data to API.
* **APIs Access for External Applications**: 
   * Upon specific authorizations third parties will be enabled to acces real time data form the DSE with given credentials
* **ACG Frontend**:
  * Vendors' Machines management:
     * Visualization and import vendors' machines operated by the contractor.
  * Machines movements:
     * Visualization of machine movents inside and outside fields.

## Functionalities

* Data harmonization done with NGSI-LD Data Models (updating Observed Data Model)​
* APIs for data access​
* End user portal that will have access to data​

## APIs

See the [APIs Repository](https://github.com/Agricolus/ACG.api).

## Frontend

See [SPEC.md](https://github.com/Agricolus/ACG.frontend).

## License

This project is licensed under the [AGPL License](https://www.gnu.org/licenses/agpl-3.0.en.html).
