```mermaid
classDiagram
	class MauiProgram {
		<<static>>
		+CreateMauiApp()
	}
	MauiProgram --> AppState : Instanctiate, AddSingleton

	class AppState {
		+List~History~ Histories
		+List~WorkType~ WorkTypes
	}

	namespace Views {
		class MainPage {
			
		}
		

		class CreateRecordPage {

		}

		class CreateTypePage {
			
		}

		class ListRecordPage {

		}

		class EditRecordPage {
		
		}

		class ListTypePage {

		}

		class EditTypePage {
		
		}
	}
	MainPage <--> ListRecordPage : Shell
	MainPage <--> ListTypePage : Shell
	ListRecordPage <--> ListTypePage : Shell
	CreateRecordPage ..> CreateTypePage : PushAsync
	ListRecordPage ..> EditRecordPage : PuchAsync
	ListTypePage ..> EditTypePage : PuchAsync
	ListTypePage ..> CreateTypePage : PuchAsync
	ListRecordPage ..> CreateRecordPage : PuchAsync
	ListRecordPage ..> CreateRecordPage : PuchAsync
	ListRecordPage --> AppState : GetHistories
	ListTypePage --> AppState : GetWorkTypes

	namespace Controllers {
		class HistoryManager {
			+UpdateHistories()
			+CreateHistory()
		}

		class WorkTypeManager {
			+UpdateWorkTypes()
			+CreateWorkType()
		}
	}
	HistoryManager --> DatabaseConnection : GetHistory
	HistoryManager --> AppState : UpdateHistories
	WorkTypeManager --> DatabaseConnection : GetWorkTypes
	WorkTypeManager --> AppState : UpdateWorkTypes

	namespace Models {
		class DatabaseConnection {
			<<static>>
			+GetHistories(int limit, int offset) List~History~
			+GetWorkTypes(int limit, int offset) List~WorkType~
			+CreateHistory(History history)
			+CreateWorkType(WorkType workType)
		}

		class SupabaseService {			
			-HttpClient _httpClient
			+SupabaseConnection(HttpClient httpClient, string url, string key)
			+GetAsync~T~()
			+PostAsync~T~()
			+PatchAsync~T~()
			+DeleteAsync~T~()
		}

		class WorkType {
			+long Id
			+Names Names
			+Descriptions Descriptions
		}

		class Names {
			+string En
			+string Ja
		}

		class Descriptions {
			+string En
			+string Ja
		}
		
		class History {
			+long Id
			+long Type_id
			+Titles Titles
			+Comments Comments
			+List~ImagePath~ Images
			+string Timezone
			+datetime Uploaded
			+datetime Started
			+int Duration_minutes
			+int Break_minutes
		}

		class Titles {
			+string En
			+string Ja
		}

		class Comments {
			+string En
			+string Ja
		}

		class ImagePath {
			+string Path
		}
	}
	DatabaseConnection --o SupabaseService
	DatabaseConnection --* History : JsonDeserialize
	DatabaseConnection --* WorkType : JsonDeserialize
	WorkType -- Names
	WorkType -- Descriptions
	History -- Titles
	History -- Comments
	History -- ImagePath
```