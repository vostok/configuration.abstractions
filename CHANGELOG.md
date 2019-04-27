## 0.1.8 (27-04-2019): 

* Allowed to apply SecretAttribute to classes and interfaces.

## 0.1.7 (26-04-2019): 

* Implemented 'PerElement' array merge style (https://github.com/vostok/configuration/issues/16).

## 0.1.6 (19-03-2019): 

* `BindByAttribute`, `ValidateByAttribute` and `RequiredByDefaultAttribute` can now be applied to interfaces.

## 0.1.5 (01-03-2019): 

* Minor fixes in string representation of tree nodes.

## 0.1.4 (28-02-2019): 

* ArrayNode, ObjectNode: merge no longer prefers to replace when node names differ.

## 0.1.3 (28-02-2019): 

* IConfigurationProvider: removed ObserveWithErrors() methods.
* IConfigurationProvider: sequences returned by Observe() methods are now guaranteed not to produce OnError notifications.

## 0.1.2 (27-02-2019): 

* ISettingsBinder: settings argument can be null.

## 0.1.1 (26-02-2019): 

* Added BindByAttribute to provide custom binders for fields, properties and types.
* Removed non-generic version of ISettingsBinder.
* Implemented ObjectNodeBuilder for efficient ObjectNode mutation.
* Implemented JSON-like ToString() for all settings nodes.

## 0.1.0 (18-02-2019): 

Initial prerelease.