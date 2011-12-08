namespace ChirpyTest.ContainerSpecs
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition.Hosting;
	using System.Linq;
	using Chirpy;
	using Chirpy.Exports;
	using ChirpyInterface;
	using Machine.Specifications;

	public class When_composing_a_Chirp_object
	{
		static Chirp chirp;
		static CompositionContainer compositionContainer;
		static ChirpyEngineResolver engineResolver;
		static IEnumerable<Type> engines;

		Establish context = () =>
			{
				var assembly = typeof (Chirp).Assembly;

				var catelog = new AssemblyCatalog(assembly);

				compositionContainer = new CompositionContainer(catelog);

				engines = assembly.GetExportedTypes()
					.Where(t => typeof (IChirpyEngine).IsAssignableFrom(t))
					.Where(t => !t.IsAbstract)
					.Where(t => t != typeof (LazyMefEngine));
			};

		Because of = () =>
			{
				compositionContainer.Compose(new CompositionBatch());

				chirp = compositionContainer.GetExportedValue<Chirp>();

				engineResolver = chirp.EngineResolver as ChirpyEngineResolver;
			};

		It the_EngineResolver_should_not_be_null = () => chirp.EngineResolver.ShouldNotBeNull();
		It the_FileHandler_should_not_be_null = () => chirp.FileHandler.ShouldNotBeNull();
		It the_TaskList_should_not_be_null = () => chirp.TaskList.ShouldNotBeNull();
		It the_ProjectItemManager_should_not_be_null = () => chirp.ProjectItemManager.ShouldNotBeNull();

		It the_dotless_engine_should_exist = () => chirp.EngineResolver.GetEngineByName("Dotless").ShouldNotBeNull();
		It the_config_engine_should_exist = () => chirp.EngineResolver.GetEngineByName("Config").ShouldNotBeNull();
		It the_YuiCssCompressor_engine_should_exist = () => chirp.EngineResolver.GetEngineByName("YUI CSS Compressor").ShouldNotBeNull();

		It all_engines_should_be_exported = () => engineResolver.Engines.Select(e => e.Value.GetType()).ShouldContain(engines);
	}
}