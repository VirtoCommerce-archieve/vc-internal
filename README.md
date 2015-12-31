Virto Commerce Internal
============
Virto Commerce Internal is number extensions of Virto Commerce 2.x. 
The repository is a fork of Virto Commerce <a href="https://github.com/VirtoCommerce/vc-internal#virto-commerce-2x">repository</a>.

#How to update to last Virto Commerce version
<ol>
<li>Checkout the repository</li>
<li>Set vc-community as upstream repository. Use command: ”git remote add upstream https://github.com/VirtoCommerce/vc-community.git"</li>
<li>Fetch from upstream repository “git fetch upstream”</li>
<li>Merge “git merge upstream/BranchName” (BranchName  is a branch you want to merge)</li>
<li>Push Merge to the repository</li>
</ol>

#How to change or fix something?
<ol>
<li>Update to Virto Commerce last version</li>
<li>Make chenges</li>
<li>Update version in "module.manifest" file</li>
<li>Commit changes</li>
</ol>

#How to make packages for Virto Commerce app store?
<ol>
<li>Execute "pack_modules.cmd" file in the root of repository to generate packages</li>
<li>Go to "\artifacts\modules" folder</li>
<li>Look for ModuleID_x.x.x.zip. (x.x.x is version into "module.manifest" file)</li>
</ol>

#How to install module package to Virto Commerce manager?
<ol>
<li>Open app store manager.</li>
<li>Go to Configuration\Modules.</li>
<li>Click "Install" toolbar command.</li>
<li>In the opened blade, pick up a package.</li>
<li>Click “Install” button.</li>
<li>At the end, click “Restart” button.</li>
</ol>

#How to update “Modules Publishing” module?
<ol>
<li>Open app store manager.</li>
<li>Go to Configuration\Modules.</li>
<li>Select updated module.</li>
<li>In the opened blade, click “Update” toolbar command.</li>
<li>In the opened blade, pick up a new version module package.</li>
<li>Click “Update” button.</li>
<li>At the end, click “Restart” button.</li>
</ol>
