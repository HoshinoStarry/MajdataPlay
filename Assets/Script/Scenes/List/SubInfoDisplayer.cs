using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MajdataPlay.Types;
using Cysharp.Threading.Tasks;
using MajdataPlay.Net;
using System.Text.Json;
using System.Threading;
using MajdataPlay.Utils;
using System.Threading.Tasks;
#nullable enable
namespace MajdataPlay.List
{
    public class SubInfoDisplayer : MonoBehaviour
    {
        public TMP_Text id_text;
        public TMP_Text good_text;
        public TMP_Text CommentText;
        public GameObject CommentBox;
        public GameObject llxb;

        CancellationTokenSource _cts = new();

        // Start is called before the first frame update
        public void RefreshContent(ISongDetail detail)
        {
            if (detail is OnlineSongDetail onlineDetail)
            {
                id_text.text = "ID: " + onlineDetail.Id;
                _cts.Cancel();
                _cts = new();
                GetOnlineInteraction(onlineDetail, _cts.Token).Forget();
                llxb.SetActive(true);
            }
            else
            {
                id_text.text = "";
                good_text.text = "";
                _cts.Cancel();
                CommentBox.SetActive(false);
                llxb.SetActive(false);
            }
        }

        async UniTaskVoid GetOnlineInteraction(OnlineSongDetail song, CancellationToken token = default)
        {
            await UniTask.SwitchToThreadPool();
            var client = HttpTransporter.ShareClient;
            var interactUrl = song.ServerInfo.Url + "/maichart/" + song.Id + "/interact";
            using var rsp = await client.GetAsync(interactUrl, token);
            using var intjson = await rsp.Content.ReadAsStreamAsync();
            var list = await Serializer.Json.DeserializeAsync<MajNetSongInteract>(intjson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            await UniTask.Yield(cancellationToken: token);
            token.ThrowIfCancellationRequested();
            good_text.text = "��: " + list.Plays + " ��: " + list.Likes.Length + " ��: " + list.Comments.Length;

            CommentBox.SetActive(true);
            foreach (var comment in list.Comments)
            {
                var text = comment.Sender.Username + "˵��\n" + comment.Content + "\n";
                CommentText.text = text;
                await UniTask.Delay(5000, cancellationToken: token);
                token.ThrowIfCancellationRequested();
            }
            CommentBox.SetActive(false);
        }
    }
}