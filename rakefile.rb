load 'rakeconfig.rb'
$MSBUILD15CMD = MSBUILD15CMD.gsub(/\\/,"/")

task :continuous, [:config] => [:assemblyinfo, :build, :tests]

task :release, [:config] => [:assemblyinfo, :deploy, :pack]

task :restorepackages do
    sh "nuget restore #{SOLUTION}"
end

task :build, [:config] => :restorepackages do |msb, args|
	sh "\"#{$MSBUILD15CMD}\" #{SOLUTION} \/t:Clean;Build \/p:Configuration=#{args.config}"
end

task :tests do 
	sh 'dotnet test --logger:"nunit;LogFilePath=test-result.xml"'
end

task :deploy, [:config] => :restorepackages do |msb, args|
	args.with_defaults(:config => :Release)
	sh "\"#{$MSBUILD15CMD}\" #{SOLUTION} \/t:Clean;Build \/p:Configuration=#{args.config}"
end

desc "Sets the version number from SharedAssemblyInfo file"    
task :assemblyinfo do 
	asminfoversion = File.read("SharedAssemblyInfo.cs").match(/AssemblyInformationalVersion\("(\d+)\.(\d+)\.(\d+)(-.*)?"/)
    
	puts asminfoversion.inspect
	
    major = asminfoversion[1]
	minor = asminfoversion[2]
	patch = asminfoversion[3]
    suffix = asminfoversion[4]
	
	version = "#{major}.#{minor}.#{patch}"
    puts "version: #{version}#{suffix}"
    
	# DO NOT REMOVE! needed by build script!
    f = File.new('version', 'w')
    f.write "#{version}#{suffix}"
    f.close
    # ----
end

desc "Pushes the plugin packages into the specified folder"    
task :pack, [:config] do |t, args|
	args.with_defaults(:config => :Release)
    Dir.chdir('DicomTypeTranslation') do
		
		version = File.open('version') {|f| f.readline}
		puts "version: #{version}"
	
        sh "nuget pack HIC.DicomTypeTranslation.nuspec -Properties Configuration=#{args.config} -IncludeReferencedProjects -Symbols -Version #{$VERSION}#{$SUFFIX}"
        sh "nuget push HIC.DicomTypeTranslation.#{$VERSION}#{$SUFFIX}.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey #{NUGETKEY}"
    end
end
