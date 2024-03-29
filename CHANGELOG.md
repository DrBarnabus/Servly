# Changelog

All notable changes to this project will be automatically documented in this file.


### [0.2.1](https://github.com/DrBarnabus/Servly/compare/v0.2.0...v0.2.1) (2022-08-04)

_Changes to build and distribution only..._

## [0.2.0](https://github.com/DrBarnabus/Servly/compare/v0.1.0...v0.2.0) (2022-08-04)


### Features

* **core:** add IChainedServlyBuilder and Instanced Options ([c88d018](https://github.com/DrBarnabus/Servly/commit/c88d018e3c3f9114c69912089e957493b6d7a02d)), closes [#16](https://github.com/DrBarnabus/Servly/issues/16) [#16](https://github.com/DrBarnabus/Servly/issues/16)
* **hosting:** add AspNetCore support and ConfigureWebHost ([7fac1f7](https://github.com/DrBarnabus/Servly/commit/7fac1f7ad51d6f4f5f6b040a79735d0c071b00d0))
* **hosting:** add ConfigureModules to ServlyHostBuilder ([f28ed96](https://github.com/DrBarnabus/Servly/commit/f28ed967f8fb01599e6bc3d5cbcc18e73629adca))
* **hosting:** implement ServlyHostBuilder and ServlyHost ([dd70f55](https://github.com/DrBarnabus/Servly/commit/dd70f55973a58f0746069df1e4037e14f5fe7765))
* **idempotency:** implemented updated Redis Persistence Pattern and HSET ([b7f561c](https://github.com/DrBarnabus/Servly/commit/b7f561c4b3a61f5db9cc55ba61e8a40a2d4444ab)), closes [#14](https://github.com/DrBarnabus/Servly/issues/14) [#14](https://github.com/DrBarnabus/Servly/issues/14)
* **persistence.redis:** re-implement RedisPersistence layer with instance support ([6712b61](https://github.com/DrBarnabus/Servly/commit/6712b619aa99eb7650ac43891b644c145f3756ec)), closes [#13](https://github.com/DrBarnabus/Servly/issues/13) [#13](https://github.com/DrBarnabus/Servly/issues/13)


### Bug Fixes

* **core:** fix ServlyBuilderTests for AddOptions SectionKey validation ([03b0f79](https://github.com/DrBarnabus/Servly/commit/03b0f79ba646f8ff2274fdaf9973b4d5bacd19f4))
* **hosting:** correctly stop host on WaitForShutdownAsync ([4fc23ad](https://github.com/DrBarnabus/Servly/commit/4fc23ad32f562c1da5cd86acbb0b0f0fcf165ca4))
* **samples:** fix Hosting Sample launchSettings.json ([0e02489](https://github.com/DrBarnabus/Servly/commit/0e02489c7cb474b8eaaae75342eb206a79b39acf))

## 0.1.0 (2022-03-15)


### Features

* **all:** unify extensions namespace for application building extensions ([9c0a174](https://github.com/DrBarnabus/Servly/commit/9c0a174aff06c0bba5225528fffee2e0671e0380))
* **authentication:** add AspNetCore middleware to set AuthenticationContext from Authentication ([ce9b7ee](https://github.com/DrBarnabus/Servly/commit/ce9b7ee69695394de15bf4671e034bb0646be0ab))
* **authentication:** add AuthenticationContext to pass authentication details into Dependency Injection ([142e9b0](https://github.com/DrBarnabus/Servly/commit/142e9b0c2d43030213c30701d136270be7651f8c))
* **authentication:** add ClaimsPrincipal Extension Methods to access Claims ([8b443ad](https://github.com/DrBarnabus/Servly/commit/8b443ad6aed655262eef06f836312a9d139d6fb1))
* **authentication:** implement ServlyBuilder for type registration ([1f2dbab](https://github.com/DrBarnabus/Servly/commit/1f2dbab865133a804b0063c9627867f941ac914c))
* **core:** add Guard class for future use ([a21562d](https://github.com/DrBarnabus/Servly/commit/a21562d8622ff8adbb6c4e8dd9e76f6cc53afa0d))
* **core:** add IClock interface for Unit Testing ([c290c98](https://github.com/DrBarnabus/Servly/commit/c290c98a48ff90d94a79e591b07409f0e192cd65))
* **core:** add ServlyBuilder class for adding Servly Modules to Applications ([#5](https://github.com/DrBarnabus/Servly/issues/5)) ([f4ec592](https://github.com/DrBarnabus/Servly/commit/f4ec59291fe428d253e02d65f66644d77f5a6c04))
* **core:** add ServlyException class for future use ([b1cf8f1](https://github.com/DrBarnabus/Servly/commit/b1cf8f11e20eb34f35f3ed04749a97b4c7bc639f))
* **core:** add Utilities class for future use ([ea082fa](https://github.com/DrBarnabus/Servly/commit/ea082fae143480af94e9c3be6db1ef9f83eedb66))
* **core:** register SystemClock IClock by default ([892d974](https://github.com/DrBarnabus/Servly/commit/892d97442f90cc9486329e43d68a37f657ef9122))
* **idempotency:** add AspNetCore middleware for implementing idempotent requests ([b04f91a](https://github.com/DrBarnabus/Servly/commit/b04f91a47b939801d588885b9b4e7151f1be202f))
* **idempotency:** implement ServlyBuilder for type registration ([0571a40](https://github.com/DrBarnabus/Servly/commit/0571a40e60de02b791031ee0c689ae1b5bb61b0a))
* **modelbinding.hybrid:** add AspNetCore ModelBinder for Hybrid Sources ([1091633](https://github.com/DrBarnabus/Servly/commit/1091633ede49d2372e2177656d6808f5c1267ee3))
* **persistence.redis:** implement ServlyBuilder for type registration ([561251c](https://github.com/DrBarnabus/Servly/commit/561251c98330ea34ccec989f7c9705f1694d703c))


### Bug Fixes

* **authentication:** stop double dispose of authentication context state ([46c53f0](https://github.com/DrBarnabus/Servly/commit/46c53f0219d1deefe5dd249bf7bf6987896e7e79))
* **core:** fix for GuardInterpolatedStringHandler always appending ([31a0516](https://github.com/DrBarnabus/Servly/commit/31a051600562a8c2881169640913f63986691560))
