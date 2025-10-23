### Prompt (English, restructured)

To demonstrate the functionality within a larger series, a demo project with a simple test shall be created.

The demo will be based on an HTML page that allows navigation between two subpages via a menu.

### Architecture Requirements

The project should be structured using the following objects:

* Tab Object (T1)
* Page Objects for the entire web page
  * P1 (child of T1)
* Page Object for the first subpage
  * (child of P1)
* Page Object for the second subpage
  * (child of P1)
* Control Object for the menu

### Functional Requirements

* Each page should execute a visible UI action, e.g., clicking a checkbox.
* Each page should display a different control type.
* Existing Control Objects from the “Controls” project shall be reused.
* Each Page Object must have its own interface.
* The test shall only use these interfaces.

### Relationship Rules

* The two Page Objects must not have a direct *child-of* relationship to the Tab Object.
* This relationship is instead dynamically registered in the test using a naming convention, which must be documented in the text.
* The purpose is to demonstrate that Page Objects can originate from different modules that are unaware of the main page—reflecting real-world team separation in web projects.
* This concept must be explicitly mentioned in the code.

### Navigation Behavior

* In the test, navigation between pages shall be done using a `goto` method.
* Since the top-level Page Object does not know its child pages, the `goto<T>` method should perform the click based on the type name `T`.

### Implementation Plan

1. Implement the above architecture and behavior completely.
2. Ensure the test runs successfully.
3. Create all files inside a new folder:

   ```
   Trumpf.Coparoo.Playwright.Demo
   ```

   with a new `.csproj` project.
4. The test should have two variants:
   * Headless mode (for CI pipeline)
   * Visible browser mode (to observe interactions)
   * Only the headless version runs in the CI pipeline.
5. Organize the folder structure as follows:
   * Separate Tab, Page, and Control objects into distinct folders.
   * Separate interfaces and implementations into dedicated folders.
6. Follow Clean Code principles and target the latest C# language version.
7. Place the `demo.html` file inside a `wwwroot` folder.
8. Create a README.md (in English) describing the key ideas of the demo.
9. The project will be publicly published on GitHub, so it must be concise, clean, and of high quality.