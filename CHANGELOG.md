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