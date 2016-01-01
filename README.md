Virto Commerce Internal
============
Virto Commerce Internal includes several internal modules on the basis of Virto Commerce 2.x. 
The repository is a fork of <a href="https://github.com/VirtoCommerce/vc-community">vc-community</a>.

#How to update the fork to the latest of Virto Commerce 2.x version?
<ol>
<li>Set vc-community as "upstream" repository: git remote add upstream https://github.com/VirtoCommerce/vc-community.git</li>
<li>Fetch from upstream repository: git fetch upstream</li>
<li>Merge upstream repository: git merge upstream/BranchName (BranchName is a branch of vc-community you want to merge).</li>
<li>Update platforVersion in "module.manifest" files to current platform version.</li>
<li>Push to the repository: git push</li>
</ol>

#How to change or fix something in the internal modules?
<ol>
<li>Clone the the repository.</li>
<li>Update to the latest of Virto Commerce 2.x version (optionally).</li>
<li>Make changes.</li>
<li>Update version in the "module.manifest" file of changed module(s).</li>
<li>Push to the repository.</li>
</ol>

#How to make modules packages?
<ol>
<li>Clone the repository.</li>
<li>Go to root folder of local repository.</li>
<li>Execute "pack_modules.cmd" file to generate modules packages.</li>
<li>Go to the "artifacts\modules" folder. The folder contains a number of modules packages. Version of module package depending of version in "module.manifest" file.</li>
</ol>

#How to add a module package to the Virto Commerce manager?
<ol>
<li>Open the Virto Commerce manager.</li>
<li>Go to Configuration\Modules.</li>
<li>Click on the "Install" toolbar command.</li>
<li>In the opened blade, pick up a package of module you want to install.</li>
<li>Click on the "Install" button.</li>
<li>At the end, click on the "Restart button.</li>
</ol>

#How to update Virto Commerce manager with a module package?
<ol>
<li>Open the Virto Commerce manager.</li>
<li>Go to Configuration\Modules.</li>
<li>Select an existing module to update.</li>
<li>In the opened blade, click on the "Update" toolbar command.</li>
<li>In the opened blade, pick up a package of new version module.</li>
<li>Click on the "Update" button.</li>
<li>At the end, click on the "Restart" button.</li>
</ol>
