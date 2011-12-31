namespace ChirpyTest.TaskListSpecs
{
	using ChirpyInterface;
	using Machine.Specifications;
	using Microsoft.VisualStudio.Shell;
	using Microsoft.VisualStudio.Shell.Interop;
	using It = Machine.Specifications.It;

	class When_adding_an_error : TaskList_context
	{
		Because of = () => TaskList.Add("Error", "error.txt", "Error on this line!", 3, 5, ErrorCategory.Error);

		It should_find_the_ProjectItem = () =>
			SolutionMock.Verify(s => s.FindProjectItem("error.txt"));

		It should_attempt_to_resolve_the_IVsSolution_service = () =>
			ServiceProviderMock.Verify(p => p.GetService(typeof (IVsSolution)));

		It should_get_the_UniqueName_of_the_project = () =>
			ProjectMock.VerifyGet(p => p.UniqueName);

		It should_get_the_VsHierarchy_for_the_project = () =>
			{
				IVsHierarchy hierarchy;
				VsSolutionMock.Verify(s => s.GetProjectOfUniqueName("DemoProject", out hierarchy));
			};

		It should_call_ErrorListProvider_AddTask = () =>
			ErrorListProviderMock.Verify(p => p.AddTask(Moq.It.IsAny<ErrorTask>()));

		It should_have_1_task = () =>
			TaskList.Tasks.Count.ShouldEqual(1);

		It should_add_to_TaskProjects_dictionary = () =>
			TaskList.TaskProjects[TaskList.Tasks[0]].ShouldBeTheSameAs(ProjectMock.Object);

		It the_error_message_should_be_correct = () =>
			TaskList.Tasks[0].Text.ShouldEqual("Error\nError on this line!");

		It the_file_name_should_be_correct = () =>
			TaskList.Tasks[0].Document.ShouldEqual("error.txt");

		It the_category_should_be_error = () =>
			TaskList.Tasks[0].ErrorCategory.ShouldEqual(TaskErrorCategory.Error);

		It the_line_should_be_3 = () =>
			TaskList.Tasks[0].Line.ShouldEqual(3);

		It the_column_should_be_5 = () =>
			TaskList.Tasks[0].Column.ShouldEqual(5);

		It the_hierarchy_should_be_correct = () =>
			TaskList.Tasks[0].HierarchyItem.ShouldBeTheSameAs(VsHierarchyMock.Object);

		It should_activate_the_error_list_window = () => ErrorWindowMock.Verify(w => w.Activate());
	}
}
