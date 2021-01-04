﻿using ATA.Library.Shared.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace ATA.Library.Client.Web.UI.Shared
{
    public partial class TopNavBar
    {
        private string _userDropdownCssClass;
        private string _userName;
        private string _personnelCode;
        private string _userProfileImageAddress;

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateTask;

            _userName = authState.User.Identity!.Name;

            var userPersonnelCodeClaim = authState.User.Claims.Single(c => c.Type == AppStrings.Claims.PersonnelCode);

            _personnelCode = userPersonnelCodeClaim.Value;

            _userProfileImageAddress = $"http://cdn.app.ataair.ir/img/pers/{_personnelCode}.png";

        }

        private void OnUserProfileClick()
        {
            _userDropdownCssClass = string.IsNullOrWhiteSpace(_userDropdownCssClass) ? "open" : string.Empty;
        }

        private void OnUserImageLoadFailed()
        {
            _userProfileImageAddress = "/images/ata-layout/user-default.jpg";
        }
    }
}
