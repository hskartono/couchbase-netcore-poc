using AppCoreApi.ApplicationCore.Entities;
using Xunit;

namespace UnitTest.Entities
{
	public class KuisionerTest
	{
		[Fact]
		public void can_add_new_item()
		{
			var kuisioner = new Kuisioner();
			Assert.Equal(0, kuisioner.KuisionerDetail.Count);
			kuisioner.AddKuisionerDetail("1", "sample4", "sample5", "sample6", "sample7", 8);
			Assert.Equal(1, kuisioner.KuisionerDetail.Count);

			var newItemKuisionerDetail = new KuisionerDetail("1", "sample4", "sample5", "sample6", "sample7", 8, kuisioner) { Id = 1 };
			kuisioner.AddOrReplaceKuisionerDetail(newItemKuisionerDetail);
			Assert.Equal(2, kuisioner.KuisionerDetail.Count);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.Id == newItemKuisionerDetail.Id);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.KontenSoal == newItemKuisionerDetail.KontenSoal);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.Pilihan1 == newItemKuisionerDetail.Pilihan1);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.PIlihan2 == newItemKuisionerDetail.PIlihan2);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.Pilihan3 == newItemKuisionerDetail.Pilihan3);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.KunciJawaban == newItemKuisionerDetail.KunciJawaban);

		}

		[Fact]
		public void can_replace_existing_item()
		{
			var kuisioner = new Kuisioner();
			Assert.Equal(0, kuisioner.KuisionerDetail.Count);
			var newItemKuisionerDetail = new KuisionerDetail("1", "sample4", "sample5", "sample6", "sample7", 8, kuisioner) { Id = 1 };
			kuisioner.AddOrReplaceKuisionerDetail(newItemKuisionerDetail);
			Assert.Equal(1, kuisioner.KuisionerDetail.Count);

			var repalaceItemKuisionerDetail = new KuisionerDetail("1", "replace sample4", "replace sample5", "replace sample6", "replace sample7", 89, kuisioner) { Id = 1 };
			kuisioner.AddOrReplaceKuisionerDetail(repalaceItemKuisionerDetail);
			Assert.Equal(1, kuisioner.KuisionerDetail.Count);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.Id == repalaceItemKuisionerDetail.Id);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.KontenSoal == repalaceItemKuisionerDetail.KontenSoal);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.Pilihan1 == repalaceItemKuisionerDetail.Pilihan1);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.PIlihan2 == repalaceItemKuisionerDetail.PIlihan2);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.Pilihan3 == repalaceItemKuisionerDetail.Pilihan3);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.KunciJawaban == repalaceItemKuisionerDetail.KunciJawaban);

		}

		[Fact]
		public void can_remove_existing_item()
		{
			var kuisioner = new Kuisioner();
			Assert.Equal(0, kuisioner.KuisionerDetail.Count);
			var newItemKuisionerDetail = new KuisionerDetail("1", "sample4", "sample5", "sample6", "sample7", 8, kuisioner) { Id = 1 };
			kuisioner.AddOrReplaceKuisionerDetail(newItemKuisionerDetail);
			Assert.Equal(1, kuisioner.KuisionerDetail.Count);

			kuisioner.RemoveKuisionerDetail(newItemKuisionerDetail);
			Assert.Equal(0, kuisioner.KuisionerDetail.Count);
			Assert.DoesNotContain(kuisioner.KuisionerDetail, e => e.Id == newItemKuisionerDetail.Id);
			Assert.DoesNotContain(kuisioner.KuisionerDetail, e => e.KontenSoal == newItemKuisionerDetail.KontenSoal);
			Assert.DoesNotContain(kuisioner.KuisionerDetail, e => e.Pilihan1 == newItemKuisionerDetail.Pilihan1);
			Assert.DoesNotContain(kuisioner.KuisionerDetail, e => e.PIlihan2 == newItemKuisionerDetail.PIlihan2);
			Assert.DoesNotContain(kuisioner.KuisionerDetail, e => e.Pilihan3 == newItemKuisionerDetail.Pilihan3);
			Assert.DoesNotContain(kuisioner.KuisionerDetail, e => e.KunciJawaban == newItemKuisionerDetail.KunciJawaban);

		}

		[Fact]
		public void remove_nont_exist_item_should_return_nothing()
		{
			var kuisioner = new Kuisioner();
			Assert.Equal(0, kuisioner.KuisionerDetail.Count);
			var newItemKuisionerDetail = new KuisionerDetail("1", "sample4", "sample5", "sample6", "sample7", 8, kuisioner) { Id = 1 };
			kuisioner.AddOrReplaceKuisionerDetail(newItemKuisionerDetail);
			Assert.Equal(1, kuisioner.KuisionerDetail.Count);

			var removeItemKuisionerDetail = new KuisionerDetail("1", "sample4", "sample5", "sample6", "sample7", 8, kuisioner) { Id = 2 };
			kuisioner.RemoveKuisionerDetail(removeItemKuisionerDetail);
			Assert.Equal(1, kuisioner.KuisionerDetail.Count);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.Id == newItemKuisionerDetail.Id);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.KontenSoal == newItemKuisionerDetail.KontenSoal);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.Pilihan1 == newItemKuisionerDetail.Pilihan1);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.PIlihan2 == newItemKuisionerDetail.PIlihan2);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.Pilihan3 == newItemKuisionerDetail.Pilihan3);
			Assert.Contains(kuisioner.KuisionerDetail, e => e.KunciJawaban == newItemKuisionerDetail.KunciJawaban);

		}

		[Fact]
		public void can_clear_existing_items()
		{
			var kuisioner = new Kuisioner();
			Assert.Equal(0, kuisioner.KuisionerDetail.Count);
			var newItemKuisionerDetail = new KuisionerDetail("1", "sample4", "sample5", "sample6", "sample7", 8, kuisioner) { Id = 1 };
			kuisioner.AddOrReplaceKuisionerDetail(newItemKuisionerDetail);
			Assert.Equal(1, kuisioner.KuisionerDetail.Count);

			kuisioner.ClearKuisionerDetails();
			Assert.Equal(0, kuisioner.KuisionerDetail.Count);

		}
	}
}
