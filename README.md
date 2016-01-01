Virto Commerce Internal
============
Virto Commerce Internal includes several internal modules on the basis of Virto Commerce 2.x. 
The repository is a fork of <a href="https://github.com/VirtoCommerce/vc-community">vc-community</a>.

#How to update to the lastest Virto Commerce 2.x version?
<ol>
<li>Set vc-community as upstream repository: "git remote add upstream https://github.com/VirtoCommerce/vc-community.git"</li>
<li>Fetch from upstream repository: "git fetch upstream"</li>
<li>Merge upstream repository :"git merge upstream/BranchName" (BranchName  is a branch of vc-community you want to merge)</li>
<li>Push to the repository: "git push"</li>
</ol>

#How to change or fix something in the internal modules?
<ol>
<li>Clone the repository</li>
<li>Update to the lastest Virto Commerce 2.x version (optionally)</li>
<li>Make chenges</li>
<li>Update version in "module.manifest" file of changed module(s)</li>
<li>Commit changes</li>
</ol>

#How to make packages for Virto Commerce app store?
<ol>
<li>Go to repository root folder</li>
<li>Execute "pack_modules.cmd" file in the root of the repository to generate packages</li>
<li>Go to "artifacts\modules" folder</li>
<li>Look for ModuleID_x.x.x.zip. (x.x.x is version stored in "module.manifest" file)</li>
</ol>

#How to install a module package to Virto Commerce manager?
<ol>
<li>Open app store manager.</li>
<li>Go to Configuration\Modules.</li>
<li>Click "Install" toolbar command.</li>
<li>In the opened blade, pick up a module package you want.</li>
<li>Click “Install” button.</li>
<li>At the end, click “Restart” button.</li>
</ol>

#How to update “Modules Publishing” module?
<ol>
<li>Open app store manager.</li>
<li>Go to Configuration\Modules.</li>
<li>Select an updated module.</li>
<li>In the opened blade, click “Update” toolbar command.</li>
<li>In the opened blade, pick up a new version module package.</li>
<li>Click “Update” button.</li>
<li>At the end, click “Restart” button.</li>
</ol>
