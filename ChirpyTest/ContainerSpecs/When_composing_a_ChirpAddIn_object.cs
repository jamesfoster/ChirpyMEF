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
	using System.ComponentModel.Composition;

	[Subject(typeof(Chirp))]
	public class When_composing_a_ChirpAddIn_object : AddIn_context
	{
		static ChirpyAddIn chirpAddIn;
		static Chirp chirp;
		static CompositionContainer compositionContainer;
		static EngineResolver engineResolver;
		static IEnumerable<Type> engines;

		Establish context = () =>
			{
				var assembly = typeof (ChirpyAddIn).Assembly;

				var catelog = new AssemblyCatalog(assembly);

				compositionContainer = new CompositionContainer(catelog);

				engines = assembly.GetExportedTypes()
					.Where(t => typeof (IEngine).IsAssignableFrom(t))
					.Where(t => !t.IsAbstract)
					.Where(t => t != typeof (EngineContainer));
			};

		Because of = () =>
			{
				compositionContainer.Compose(new CompositionBatch());

				StaticPart.App = AppMock.Object;

				chirpAddIn = new ChirpyAddIn();
				
				compositionContainer.ComposeParts(chirpAddIn);

				chirp = chirpAddIn.Chirp;

				engineResolver = chirp.EngineResolver as EngineResolver;
			};

		It the_Chirp_should_not_be_null = () => chirpAddIn.Chirp.ShouldNotBeNull();
		It the_ProjectItemManager_shoud_not_be_null = () => chirpAddIn.ProjectItemManager.ShouldNotBeNull();
		It the_TaskList_should_not_be_null = () => chirpAddIn.TaskList.ShouldNotBeNull();

		It the_EngineResolver_should_not_be_null = () => chirp.EngineResolver.ShouldNotBeNull();
		It the_FileHandler_should_not_be_null = () => chirp.FileHandler.ShouldNotBeNull();
		It the_Logger_should_not_be_null = () => chirp.Logger.ShouldNotBeNull();
		It the_ExtensionResolver_should_not_be_null = () => chirp.ExtensionResolver.ShouldNotBeNull();

		It the_dotless_engine_should_exist = () => chirp.EngineResolver.GetEngineByName("Dotless").ShouldNotBeNull();
		It the_config_engine_should_exist = () => chirp.EngineResolver.GetEngineByName("Config").ShouldNotBeNull();
		It the_YuiCssCompressor_engine_should_exist = () => chirp.EngineResolver.GetEngineByName("YUI CSS Compressor").ShouldNotBeNull();

		It all_engines_should_be_exported = () => engineResolver.Engines.Select(e => e.Value.GetType()).ShouldContain(engines);
	}
}