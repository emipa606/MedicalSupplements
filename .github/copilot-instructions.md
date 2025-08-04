# GitHub Copilot Instructions for RimWorld Mod: Medical Supplements (Continued)

---

## Mod Overview and Purpose

**Medical Supplements (Continued)** is an updated version of the original mod by peladors with enhancements aimed at improving the quality and functionality of medical gameplay in RimWorld. It introduces new pharmaceuticals and food items, providing players with additional tools and strategies to manage health and diseases in their colonies. The mod enhances immersion by adding a variety of medical-related machinery, items, and procedures.

## Key Features and Systems

1. **Drug Chemical Mixer**: Automatically produces precursor chemicals. It features input and output hoppers for resources management, where players can select the chemical to produce and set stock limits.

2. **Drug Cabinet**: Functions as a shelf. It adds cleanliness and boosts drug lab production efficiency.

3. **Disinfectant Stand**: Sterilizes its surroundings, enhancing cleanliness and beauty. Essential for makeshift hospitals or food areas.

4. **Medical Mats**: Transportable bedrolls with four technological versions. Useful for field or caravan use.

5. **Precursor Chemicals**: Introduces precursors to provide gameplay depth and balance in drug production.

6. **New Drugs**: A variety of drugs are introduced, including:
   - Immunisation injections that alter immunity traits.
   - Cerebrax injections manage psychic sensitivity.
   - Battle Stims and Transcendence injections offer strategic combat enhancements.

7. **Additional Surgical Procedures**:
   - **Medical Stent**: Alleviates arterial blockages.
   - **Artificial Back Discs**: Addresses the bad back condition.
   - **Advanced Surgical Tools**: Required for complex surgeries.

8. **Mod Options**: Includes features like realistic bandage use and adjustable research cost settings.

## Coding Patterns and Conventions

- **Class Naming**: Use descriptive names that reflect the purpose, like `HediffCompProperties_MSAddict` for class files related to hediff components.
- **Method Naming**: Use camelCase for method names, ensuring clarity in purpose, such as `StartMixSustainer` or `ToggleProducing`.

## XML Integration

The mod heavily relies on XML for defining items, recipes, and other game data configurations. Ensure XML files are correctly structured to integrate seamlessly with the C# backend. XML files typically define:
- Items (including pharmaceuticals and food)
- Recipes (production processes and requirements)
- Buildings and Furnishings (like the Drug Cabinet or Disinfectant Stand)

## Harmony Patching

The mod uses Harmony for method patching, allowing dynamic method replacement or extension in the game's code. Key files involved in patching include:
- `HealthAIUtility_FindBestMedicine_PostPatch`
- `TryGiveJob_PostPatch`
- `GetBuildingResourcesLeaveCalculator_PostPatch`
- Methods in these classes may override or extend vanilla behaviors, crucial for integrating custom logic, such as deciding on the best medicine.

## Suggestions for Copilot

- **Code Completion**: Utilize Copilot to autocomplete repetitive patterns in method definitions, especially for similar behavior in different components, such as `HediffComp` derivatives.
- **Harmony Helpers**: Leverage Copilot to assist in generating IL code if deeper Harmony integration is needed, for patches that require precise control over instructions.
- **Recipe & Item Definition**: When working with XML, Copilot can aid in structuring data entries, particularly for element-based repetitive definitions like multiple precursor chemicals or drugs.

## Additional Considerations

- Always ensure compatibility with other mods by checking Harmony patches don’t conflict.
- Utilize translation keys for multilingual support, as illustrated by the integration of a German translation.
- Regularly test mod functionalities to ensure new items, recipes, and behaviors integrate properly in ongoing saves.

For further reading and a detailed understanding of the new drugs and items introduced, refer to the accompanying PDF documentation provided with the mod. Always ensure compliance with the license (CC BY-NC-SA 4.0) when distributing or modifying the mod content.
