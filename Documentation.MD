# Poe Docs

This documentation provides a comprehensive guide for Poe!
***

## Project Structure Overview

Poe follows the MVVM (Model-View-ViewModel) pattern, ensuring a separation of concerns and facilitating maintainable code:

### Data
- **Settings**
    - `config.json`: Configuration parameters and application settings.

### Helpers
- **Dependency Injection**
    - `ConfigurationService.cs`: Manages access to application settings.
    - `RichTextBoxHelper.cs`: Provides utilities for rich text operations.

### Images
- Stores icons, illustrations, and other graphic assets.

### Models
- **API**
    - `DataMuse.cs`: Interfaces with the DataMuse API.
    - `MerriamWebsterApi.cs`: Integrates with Merriam-Webster's dictionary and thesaurus APIs.
- **Document**
    - `Document.cs`: Represents the document structure and behaviors.
    - `Bookmark.cs`: Handles bookmarking within documents.
    - `Page.cs`: Manages individual pages of a document.
    - `Pagination.cs`: Pagination functionality.
    - `Editable.cs`: Editable components of the document.
    - `Text.cs`: Textual content management.
    - `Words.cs`: Word-specific operations and data.

### Themes
- `CommonStyles.xaml`: Universal styling rules.
- `GlobalStyles.xaml`: Overarching application themes.
- `MainWindowStyles.xaml`: Styles specific to the main application window.

### ViewModels
- `DocumentViewModel.cs`: Bridges the Document model with its view.
- `MainWindowViewModel.cs`: Business logic for the main window.
- `SearchResultsViewModel.cs`: Logic for displaying and interacting with search results.

### Views
- **`App.xaml`**: Global resources and styling.
- **`MainWindow.xaml`**: The application's primary user interface.
- **`SearchResultsPage.xaml`**: Interface for search outcomes and user interactions.

***


# Poe Development Roadmap and Checklist

## Phase 1: Poe Application Development
This phase focuses on developing a standalone literary rich text editor application.

### Core Editor Functionalities
- [x] Implement rich text formatting toolbar.
  - [x] Add buttons for bold, italic, and underline formatting.
  - [x] Enable font family selection and font size adjustment.
  - [ ] Incorporate text color picker.
- [ ] Implement paragraph formatting options.
  - [ ] Add buttons for text alignment: left, center, right, and justify.
  - [ ] Include line spacing and paragraph spacing adjustments.
- [ ] Implement clipboard operations.
  - [x] Add functionality for cut, copy, and paste.
  - [ ] Ensure clipboard operations support rich text.

***

### Document Handling
- [ ] Set up basic file operations.
  - [ ] Develop a new document creation process.
  - [ ] Implement document open functionality with file picker.
  - [ ] Create save and save as functionalities with file dialogs.
  - [ ] Ensure compatibility with common text file formats (.txt, .rtf, .docx).
- [ ] Develop an auto-save feature.
  - [ ] Set up a timer-based trigger for auto-save.
  - [ ] Create logic to determine when changes have occurred to initiate auto-save.
  - [ ] Allow users to configure auto-save intervals.
- [ ] Establish document version control.
  - [ ] Implement undo and redo actions.
  - [ ] Create a change history tracker for documents.
  - [ ] Design a version comparison viewer.

***

### Advanced Text Editing
- [ ] Implement syllable count per line.
  - [ ] Develop logic to analyze and count syllables in text.
  - [ ] Display syllable count in a non-obtrusive manner.
- [ ] Enhance text insertion and editing.
  - [ ] Create smooth cursor movement and text flow.
  - [ ] Ensure text reflows dynamically with edits.
- [ ] Develop rhyme scheme visualization.
  - [ ] Analyze text for rhyme patterns.
  - [ ] Graphically highlight rhyme schemes.
- [ ] Implement scansion and meter detection.
  - [ ] Create algorithms to detect poetic meter.
  - [ ] Allow users to view and edit scansion.

***

### Media and Content
- [ ] Allow image insertion into documents.
  - [ ] Add an interface for users to embed images.
  - [ ] Enable basic image formatting and resizing.
- [ ] Implement hyperlink functionality.
  - [ ] Develop logic to insert and edit hyperlinks.
  - [ ] Ensure hyperlinks are functional when documents are exported.

***

### User Interface
- [ ] Design a customizable UI theme system.
  - [ ] Develop a theme selection interface.
  - [ ] Implement light and dark mode options.
- [ ] Explore unique UI visual experiences.
  - [ ] Research radial gradient implementation in WPF.
  - [ ] Apply a radial gradient background as a theme option.

***

### Settings Configuration
- [ ] Create setting files
  - [ ] Set up config file
    - [ ] Define default settings
    - [ ] Define fields required / structure of the json config
- [ ] Detect the user's settings
  -  [ ] Real time settings synchronization 
- [ ] Application Settings
  - [ ] Keybindings
  - [ ] File location / pathing
  - [ ] Version Control / Backups
  - [ ] Repair solution
- [ ] Document Settings
  - [ ] Page Settings
    - [ ] Width
    - [ ] Height
    - [ ] Margin
    - [ ] Header
    - [ ] Footer
- [ ] UI Settings
  - [ ] Night theme
  - [ ] Customization of components
  - [ ] Color of sidebars
  - [ ] Dynamic binding...
- [ ] Language
    - https://www.youtube.com/watch?v=FJSJLf76mBM

***

### External API Integration
- [x] Integrate Datamuse API.
  - [x] Set up API requests for word suggestions and rhymes.
  - [x] Parse and display API response data within the application.
- [ ] Integrate Wordnik API.
  - [ ] Establish API requests for definitions, synonyms, and antonyms.
  - [ ] Incorporate API data into the application's dictionary and thesaurus features.

### Cross-Platform Compatibility
- [ ] Research cross-platform frameworks.
  - [ ] Evaluate AvaloniaUI for cross-platform capabilities.
  - [ ] Prototype basic application features in AvaloniaUI.

### Open-Source Libraries Exploration
- [ ] Examine OfficeIMO for potential functionalities.
  - [ ] Test OfficeIMO's compatibility with Poe's requirements.
  - [ ] Integrate OfficeIMO features that align with Poe's editing capabilities.
- [ ] Assess front-end libraries.
  - [ ] Experiment with cshtml5 for web capabilities.
  - [ ] Explore QuillJS as a potential rich text editor for web integration.

***

## Phase 2: Poe Website Development
This phase entails creating a web presence for the Poe application.

### Website Fundamentals
- [ ] Set up a basic web structure with HTML, CSS, and JavaScript.
- [ ] Outline the website content, including download links, feature lists, and support resources.

### Hosting and Domain
- [ ] Choose a hosting service and purchase a domain name.
- [ ] Ensure SSL certification for secure connections.

### Marketing and Outreach
- [ ] Develop a content marketing strategy for launch and user acquisition.
- [ ] Create promotional materials and engage with writing communities.

### Additional Features Consideration
- [ ] Implement a web-based version of Poe using technologies like Blazor or cshtml5.
- [ ] Consider integrating PoetryDB for a searchable database of poems.

## Phase 3: Expansion and Growth
This phase deals with expanding features and growing the user base.

### Engagement Features
- [ ] Implement a 'Poem of the Week' challenge with user submissions.
- [ ] Explore gamification elements like limited word challenges or poetry competitions.

### Research and Development
- [ ] Investigate self-publishing platforms for potential integration.
- [ ] Research into AI-driven features such as poem reading with distinct voices.

### Community and Support
- [ ] Set up a forum or support system for user engagement.
- [ ] Develop a system for users to provide feedback and suggestions.

### Additional Considerations
- [ ] Evaluate Git integration for version control among writers.
- [ ] Explore form helpers for generating poetic structures.

_Remember, this roadmap is iterative and may adapt as the project progresses and new opportunities or challenges arise._





***

## Dependency Injection

Dependency injection (DI) is a fundamental design pattern employed in Poe, promoting a decoupled and testable codebase.
The DI-related classes reside in the `Helpers/Dependency_Injection/` directory.

### ConfigurationService.cs

Located in `Helpers/Dependency_Injection/`, `ConfigurationService.cs` serves as a conduit to the `config.json`,
abstracting the configuration retrieval throughout the application.

### Utilizing ConfigurationService

To leverage `ConfigurationService` across Poe:

1. **Register**: Initialize and register `ConfigurationService` as a singleton within the DI container in `App.xaml.cs`.
2. **Inject**: Use constructor injection to provide `ConfigurationService` to classes, primarily ViewModels.

### Injecting ConfigurationService into a ViewModel

Below illustrates the injection of `ConfigurationService` into a ViewModel:

***

```csharp
public class ViewModelA : ObservableObject
{
    private readonly ConfigurationService _configService;

    public ViewModelA(ConfigurationService configService)
    {
        _configService = configService;
        // Access a setting from the configuration:
        var someSetting = _configService.MySetting;
    }
}
```

***

# ChatGPT Prompts

### Poe's Description / Context

Hi there, I am solo developing an application called 'Poe', a literary rich text
editor. My codebase is written in C# and WPF. My goal is to make a functional application capable of providing a MS-Word
alike experience for my users.

### Ask for Markdown

Hi there, I am solo developing an application called 'Poe', a literary
rich text editor. I have a documentation markdown page I use as I am developing the rest of the application. Can you
help me out with expanding it with this new section:

### Ask for WPF

Hi there, I am solo developing an application called 'Poe', a literary rich text
editor. My codebase is written in C# and WPF. Your task is now to implement me a new feature in my WPF project.
I would like:
